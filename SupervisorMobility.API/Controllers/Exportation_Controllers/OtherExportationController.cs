using SupervisorMobility.API.DataAccess.Services.ExportationServices;
using SupervisorMobility.API.DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.DataAccess.Entities;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Globalization;
using DocumentFormat.OpenXml.Wordprocessing;
using SupervisorMobility.API.DataAccess.Services.UserCoursesServices;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using System.Security.Principal;

namespace SupervisorMobility.API.Controllers.Exportation_Controllers
{
    [Route("api/Exportation")]
    [ApiController]
    public class OtherExportationController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _SMProcessRepository;
        private readonly ISOS_ProcessRepository _SOSProcessRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IUserCoursesServices _userCoursesService;
        private readonly ExportationStylesService stylesService;
        private readonly ExportationImgService imgService;
        private readonly ExportationSheetService sheetService;

        public OtherExportationController(ISupervisorMobilityRepository repository, IWebHostEnvironment env, ISOS_ProcessRepository SOSrepository, IUserCoursesServices userCoursesService)
        {
            _SMProcessRepository = repository;
            _SOSProcessRepository = SOSrepository;
            _env = env ?? throw new ArgumentNullException(nameof(env));
            stylesService = new ExportationStylesService();
            imgService = new ExportationImgService();
            sheetService = new ExportationSheetService();
            _userCoursesService = userCoursesService;
        }

        [HttpGet("Excel/PATYearly/{PATId}")]
        public async Task<IActionResult> PATYearlyExcelExport(int PATId)
        {
            var PAT = await _SMProcessRepository.GetPat(PATId);

            Dictionary<OperatorRole, string> roles = new Dictionary<OperatorRole, string>
            {
                { OperatorRole.SV, "SV" },
                { OperatorRole.Lider, "LIDER" },
                { OperatorRole.CA, "C/A" },
                { OperatorRole.NI, "NI" }
            };

            List<User> usersOfArea = new();
            usersOfArea.AddRange(PAT.Supervisors!);
            foreach (var item in PAT.Supervisors!)
            {
                usersOfArea.AddRange((List<User>)await _SMProcessRepository.GetAllSubordinatesAsync(item.UserId));
            }

            usersOfArea = usersOfArea.GroupBy(user => user.UserId).Select(group => group.First()).ToList();
            usersOfArea = usersOfArea.OrderBy(p => p.Payroll).ToList();

            var uniqueDistributions = usersOfArea.Where(user => user.ILURegisers != null)
                .SelectMany(user => user.ILURegisers)
                .Where(ilu => ilu.Distribution != null && ilu.AcquisitionDate.HasValue && ilu.AcquisitionDate.Value.Year == PAT.AplicationYear)
                .Select(ilu => ilu.DistributionId)
                .Distinct().ToList();


            List<Distribution> distributions = (List<Distribution>)await _SMProcessRepository.GetDistributionsForAreaAsync(PAT.AreaId.Value);


            var orderedPatDistributionComments = distributions
                .Select(id => PAT.PatDistributionComments.FirstOrDefault(pdc => pdc.DistributionId == id.DistributionId))
                .Where(pdc => pdc != null).ToList();

            var iluLevels = await _SMProcessRepository.GetAllILULevel();
            var levels = new Dictionary<string, (string, int)>();

            foreach (var iluLevel in iluLevels)
            {
                string value; int category;
                switch (iluLevel.ILULevelCode)
                {
                    case "ITrainee":
                        value = "";
                        break;
                    case "ILeader":
                    case "LTrainee":
                    case "LTraineeLeader":
                        value = "I";
                        break;
                    case "LLeader":
                    case "UTrainee":
                        value = "L";
                        break;
                    case "ULeaderTrainee":
                    case "ULeader":
                        value = "U";
                        break;
                    default:
                        value = iluLevel.ILULevelCode[0].ToString();
                        break;
                }

                category = iluLevel.ILULevelCode.Substring(1) switch
                {
                    "TraineeLeader" => 2,
                    "Trainee" => 1,
                    "Leader" => 3,
                    _ => 0
                };

                levels.Add(iluLevel.ILULevelCode, (value, category));
            }

            string templateName = "DataAccess/Templates/PAT Yearly Template.xlsx";
            const string sheetNames = "PAT Anual ";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var sheet = package.Workbook.Worksheets["PAT Anual 1"];

                //#region information table

                var InfoSup = PAT.Supervisors.FirstOrDefault();
                string ResponableSVs = string.Join(" / ", PAT.Supervisors.Select(user => user.Name));
                string ResponableSSVs = string.Join(" / ", PAT.Supervisors.Select(user => user.Superior?.Name).Where(name => name != null).Distinct());

                sheet.Cells["B4"].Value = InfoSup?.Department;
                sheet.Cells["H4"].Value = InfoSup?.Area?.Description;
                sheet.Cells["P4"].Value = InfoSup?.Group?.Description;
                sheet.Cells["X4"].Value = PAT.AplicationYear;
                sheet.Cells["AI4"].Value = ResponableSVs;
                sheet.Cells["AU4"].Value = ResponableSSVs;
                sheet.Cells["CA4"].Value = PAT.CreationDate?.ToString("dd/MM/yyyy");
                sheet.Cells["G7"].Value = PAT.KnowledgePercentage;
                sheet.Cells["G10"].Value = PAT.SaveLeader;

                var operatorsOrder = usersOfArea?.Select(p => p.UserId).ToList();

                var operatorRoles = PAT.PatUserRoles?.OrderBy(p => operatorsOrder.IndexOf(p.UserId)).ToList();

                var matrix = new ILURegister[distributions.Count, usersOfArea.Count];

                foreach (var uo in usersOfArea)
                {
                    int colIndex = usersOfArea.IndexOf(uo);
                    foreach (var reg in uo.ILURegisers)
                    {
                        if (reg.Distribution is not null)
                        {
                            int rowIndex = distributions.FindIndex(p => p.DistributionId == reg.DistributionId);
                            matrix[rowIndex, colIndex] = reg;

                        }
                    }
                }

                int Row = 12;
                string Col = "I";
                int pagePplIndex = 1;
                int pageOprIndex = 1;

                int colWidth = imgService.WidthToPixels(sheet.Column(9).Width);
                int rowHeight = imgService.HeightToPixels(50);

                for (int i = 0; i < distributions.Count; i++)
                {
                    sheet.Cells[$"B{Row}"].Value = i + 1;
                    sheet.Cells[$"C{Row}"].Value = distributions[i]?.Description;

                    string imageType = "";
                    switch (distributions[i]?.CriticalType)
                    {
                        case 1:
                            imageType = $"DataAccess/Icons/HOE_Symbols/A.png";
                            break;
                        case 2:
                            imageType = $"DataAccess/Icons/HOE_Symbols/B.png";
                            break;
                        case 3:
                            imageType = $"DataAccess/Icons/HOE_Symbols/C.png";
                            break;
                    }

                    if (imageType != "")
                    {
                        using (FileStream stream = System.IO.File.OpenRead(imageType))
                        {
                            int column = sheetService.ColumnLetterToNumber(Col);

                            var picture = sheet.Drawings.AddPicture($"{i}{distributions[i]?.CriticalType}Critical", stream);


                            picture.SetSize(rowHeight - 6, rowHeight - 6);

                            int YOffset = (rowHeight - (int)(picture.Size.Height / 9525)) / 2;
                            int XOffset = (colWidth - (int)(picture.Size.Width / 9525)) / 2;

                            picture.SetPosition(Row - 1, YOffset, column - 2, XOffset);
                        }
                    }

                    if (orderedPatDistributionComments.Any() && !string.IsNullOrEmpty(orderedPatDistributionComments[i].Comment))
                        sheet.Cells[$"CD{Row}"].Value = orderedPatDistributionComments[i]?.Comment;

                    for (int j = 0; j < usersOfArea.Count; j++)
                    {
                        if (Row == 12)
                        {
                            sheet.Cells[$"{Col}5"].Value = j + 1;
                            sheet.Cells[$"{Col}6"].Value = usersOfArea[j].Name;
                            sheet.Cells[$"{Col}10"].Value = usersOfArea[j].Payroll;
                            if (operatorRoles?.Any() ?? false)
                            {
                                sheet.Cells[$"{Col}42"].Value = operatorRoles?[j].Comment;
                                if (operatorRoles[j].Role != null)
                                    sheet.Cells[$"{Col}50"].Value = roles[operatorRoles[j].Role.Value];
                            }
                        }

                        if (matrix[i, j] == null) { Col = sheetService.GetNextCombination(Col); Col = sheetService.GetNextCombination(Col); continue; }

                        sheet.Cells[$"{Col}{Row}"].Value = matrix[i, j].AcquisitionDate?.ToString("MMM");

                        Col = sheetService.GetNextCombination(Col);

                        sheet.Cells[$"{Col}{Row}"].Value = levels[matrix[i, j].ILULevel?.ILULevelCode!].Item1;

                        string image = $"DataAccess/Icons/{matrix[i, j].ILULevel?.ILULevelCode}.png";

                        using (FileStream stream = System.IO.File.OpenRead(image))
                        {
                            int column = sheetService.ColumnLetterToNumber(Col);

                            var picture = sheet.Drawings.AddPicture($"{i}{j}Picture", stream);


                            picture.SetSize(colWidth - 6, rowHeight - 6);

                            int YOffset = (rowHeight - (int)(picture.Size.Height / 9525)) / 2;
                            int XOffset = (colWidth - (int)(picture.Size.Width / 9525)) / 2;

                            picture.SetPosition(Row - 1, YOffset, column - 1, XOffset);
                        }

                        Col = sheetService.GetNextCombination(Col);

                        if (Col == "CA")
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 0, pagePplIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];
                            }

                            sheet.Cells[$"B{Row}"].Value = i + 1;
                            sheet.Cells[$"C{Row}"].Value = distributions[i]?.Description;
                            if (!string.IsNullOrEmpty(orderedPatDistributionComments[i].Comment))
                                sheet.Cells[$"CD{Row}"].Value = orderedPatDistributionComments[i]?.Comment;

                            pagePplIndex++;
                            Col = "I";
                        }
                    }


                    Col = "I";
                    Row++;
                    if (Row > 38)
                    {
                        sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];

                        if (sheet == null)
                        {
                            sheetService.AddOtherSheet(package, 0, pagePplIndex.ToString());
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];
                        }

                        pagePplIndex++;
                        pageOprIndex = pagePplIndex;
                        Row = 12;
                    }
                    else
                    {
                        sheet = package.Workbook.Worksheets[$"{sheetNames}{pageOprIndex}"];
                        pagePplIndex = pageOprIndex;
                    }

                }

                if (!string.IsNullOrEmpty(PAT.HistoricalAbility))
                {
                    string hCol = "CD";
                    int hRow = 42;

                    dynamic data = JsonConvert.DeserializeObject(PAT.HistoricalAbility);

                    foreach (var monthData in data)
                    {
                        foreach (var month in monthData)
                        {
                            double or_o = month.First.OR_O;
                            double or_p = month.First.OR_P;

                            if (or_o != 0)
                                sheet.Cells[$"{hCol}{hRow}"].Value = or_o / 100;
                            if (or_p != 0)
                                sheet.Cells[$"{hCol}{hRow + 1}"].Value = or_p / 100;

                            hCol = sheetService.GetNextCombination(hCol);

                            if (hCol == "CJ")
                            {
                                hCol = "CD";
                                hRow = 45;
                            }
                        }
                    }
                }

                int totalPages = package.Workbook.Worksheets.Count;

                foreach (var (item, index) in package.Workbook.Worksheets.Select((item, index) => (item, index)))
                {
                    item.Cells["CH4"].Value = $"{index + 1} de {totalPages}";
                }

                //sheetService.SetPrintingOptions(package.Workbook);

                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PAT.AplicationDate.HasValue ? $"Yearly PAT {PAT.AplicationDate.Value.Year}.xlsx" : "Yearly PAT.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }

        [HttpGet("Excel/PATMonthly/{PATId}")]
        public async Task<IActionResult> PATMonthlyExcelExport(int PATId, int AplicationMonth)
        {
            var PAT = await _SMProcessRepository.GetPat(PATId);

            Dictionary<OperatorRole, string> roles = new Dictionary<OperatorRole, string>
            {
                { OperatorRole.SV, "SV" },
                { OperatorRole.Lider, "LIDER" },
                { OperatorRole.CA, "C/A" },
                { OperatorRole.NI, "NI" }
            };

            var months = new Dictionary<int, string>
            {
                { 1, "Enero" }, { 2, "Febrero" }, { 3, "Marzo" }, { 4, "Abril" },
                { 5, "Mayo" }, { 6, "Junio" }, { 7, "Julio" }, { 8, "Agosto" },
                { 9, "Septiembre" }, { 10, "Octubre" }, { 11, "Noviembre" }, { 12, "Diciembre" }
            };

            List<User> usersOfArea = new();
            usersOfArea.AddRange(PAT.Supervisors!);
            foreach (var item in PAT.Supervisors!)
            {
                usersOfArea.AddRange((List<User>)await _SMProcessRepository.GetAllSubordinatesAsync(item.UserId));
            }

            usersOfArea = usersOfArea.GroupBy(user => user.UserId).Select(group => group.First()).ToList();
            usersOfArea = usersOfArea.OrderBy(p => p.Payroll).ToList();

            var uniqueDistributions = usersOfArea.Where(user => user.ILURegisers != null)
                .SelectMany(user => user.ILURegisers)
                .Where(ilu => ilu.Distribution != null && ilu.AcquisitionDate.HasValue && ilu.AcquisitionDate.Value.Year == PAT.AplicationYear)
                .Select(ilu => ilu.DistributionId)
                .Distinct().ToList();


            List<Distribution> distributions = (List<Distribution>)await _SMProcessRepository.GetDistributionsForAreaAsync(PAT.AreaId.Value);

            //foreach (var user in usersOfArea)
            //{
            //    foreach (var reg in user.ILURegisers.Where(ilu => ilu.AcquisitionDate.HasValue && ilu.AcquisitionDate.Value.Year == PAT.AplicationYear))
            //    {
            //        if (reg.DistributionId.HasValue)
            //        {
            //            if (!distributions.Any(p => p.DistributionId == reg.DistributionId))
            //            {
            //                var dist = await _SMProcessRepository.GetDistributionOnlyIdAsync(reg.DistributionId.Value);
            //                distributions.Add(dist);
            //            }
            //        }
            //    }
            //}

            var orderedPatDistributionComments = distributions
                .Select(id => PAT.PatDistributionComments.FirstOrDefault(pdc => pdc.DistributionId == id.DistributionId))
                .Where(pdc => pdc != null).ToList();
            //foreach (var dist in uniqueDistributions) 
            //{
            //    distributions.Add();
            //}

            var iluLevels = await _SMProcessRepository.GetAllILULevel();
            var levels = new Dictionary<string, (string, int)>();

            foreach (var iluLevel in iluLevels)
            {
                string value; int category;

                switch (iluLevel.ILULevelCode)
                {
                    case "ITrainee":
                        value = "";
                        break;
                    case "ILeader":
                    case "LTrainee":
                    case "LTraineeLeader":
                        value = "I";
                        break;
                    case "LLeader":
                    case "UTrainee":
                        value = "L";
                        break;
                    case "ULeaderTrainee":
                    case "ULeader":
                        value = "U";
                        break;
                    default:
                        value = iluLevel.ILULevelCode[0].ToString();
                        break;
                }

                category = iluLevel.ILULevelCode.Substring(1) switch
                {
                    "TraineeLeader" => 2,
                    "Trainee" => 1,
                    "Leader" => 3,
                    _ => 0
                };

                levels.Add(iluLevel.ILULevelCode, (value, category));
            }

            string templateName = "DataAccess/Templates/PAT Monthly Template.xlsx";
            const string sheetNames = "PAT Mensual ";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var sheet = package.Workbook.Worksheets["PAT Mensual 1"];

                //#region information table

                var InfoSup = PAT.Supervisors.FirstOrDefault();
                string ResponableSVs = string.Join(" / ", PAT.Supervisors.Select(user => user.Name));
                string ResponableSSVs = string.Join(" / ", PAT.Supervisors.Select(user => user.Superior?.Name).Where(name => name != null).Distinct());

                sheet.Cells["B4"].Value = InfoSup?.Department;
                sheet.Cells["H4"].Value = InfoSup?.Area?.Description;
                sheet.Cells["P4"].Value = InfoSup?.Group?.Description;
                sheet.Cells["X4"].Value = months[AplicationMonth];
                sheet.Cells["AI4"].Value = ResponableSVs;
                sheet.Cells["AU4"].Value = ResponableSSVs;
                sheet.Cells["CA4"].Value = PAT.CreationDate?.ToString("dd/MM/yyyy");
                sheet.Cells["G7"].Value = PAT.KnowledgePercentage;
                sheet.Cells["G10"].Value = PAT.SaveLeader;

                var operatorsOrder = usersOfArea?.Select(p => p.UserId).ToList();

                var operatorRoles = PAT.PatUserRoles?.OrderBy(p => operatorsOrder.IndexOf(p.UserId)).ToList();

                var matrix = new ILURegister[distributions.Count, usersOfArea.Count];

                foreach (var uo in usersOfArea)
                {
                    int colIndex = usersOfArea.IndexOf(uo);
                    foreach (var reg in uo.ILURegisers)
                    {
                        if (reg.Distribution is not null)
                        {
                            int rowIndex = distributions.FindIndex(p => p.DistributionId == reg.DistributionId);
                            matrix[rowIndex, colIndex] = reg;
                        }
                    }
                }

                int Row = 12;
                string Col = "I";
                int pagePplIndex = 1;
                int pageOprIndex = 1;

                int colWidth = imgService.WidthToPixels(sheet.Column(9).Width);
                int rowHeight = imgService.HeightToPixels(50);

                for (int i = 0; i < distributions.Count; i++)
                {
                    sheet.Cells[$"B{Row}"].Value = i + 1;
                    sheet.Cells[$"C{Row}"].Value = distributions[i]?.Description;


                    string imageType = "";
                    switch (distributions[i]?.CriticalType)
                    {
                        case 1:
                            imageType = $"DataAccess/Icons/HOE_Symbols/A.png";
                            break;
                        case 2:
                            imageType = $"DataAccess/Icons/HOE_Symbols/B.png";
                            break;
                        case 3:
                            imageType = $"DataAccess/Icons/HOE_Symbols/C.png";
                            break;
                    }

                    if (imageType != "")
                    {
                        using (FileStream stream = System.IO.File.OpenRead(imageType))
                        {
                            int column = sheetService.ColumnLetterToNumber(Col);

                            var picture = sheet.Drawings.AddPicture($"{i}{distributions[i]?.CriticalType}Critical", stream);


                            picture.SetSize(rowHeight - 6, rowHeight - 6);

                            int YOffset = (rowHeight - (int)(picture.Size.Height / 9525)) / 2;
                            int XOffset = (colWidth - (int)(picture.Size.Width / 9525)) / 2;

                            picture.SetPosition(Row - 1, YOffset, column - 2, XOffset);
                        }
                    }


                    if (orderedPatDistributionComments.Any() && !string.IsNullOrEmpty(orderedPatDistributionComments[i].Comment))
                        sheet.Cells[$"CD{Row}"].Value = orderedPatDistributionComments[i]?.Comment;

                    for (int j = 0; j < usersOfArea.Count; j++)
                    {
                        if (Row == 12)
                        {
                            sheet.Cells[$"{Col}5"].Value = j + 1;
                            sheet.Cells[$"{Col}6"].Value = usersOfArea[j].Name;
                            sheet.Cells[$"{Col}10"].Value = usersOfArea[j].Payroll;
                            if (operatorRoles.Any())
                            {
                                sheet.Cells[$"{Col}42"].Value = operatorRoles?[j].Comment;
                                if (operatorRoles[j].Role != null)
                                    sheet.Cells[$"{Col}50"].Value = roles[operatorRoles[j].Role.Value];
                            }
                        }

                        if (matrix[i, j] == null) { Col = sheetService.GetNextCombination(Col); Col = sheetService.GetNextCombination(Col); continue; }

                        sheet.Cells[$"{Col}{Row}"].Value = $"{matrix[i, j].AcquisitionDate?.Day}\n-\n{matrix[i, j].EndDate?.Day}";

                        Col = sheetService.GetNextCombination(Col);

                        sheet.Cells[$"{Col}{Row}"].Value = levels[matrix[i, j].ILULevel?.ILULevelCode!].Item1;

                        string image = $"DataAccess/Icons/{matrix[i, j].ILULevel?.ILULevelCode}.png";

                        using (FileStream stream = System.IO.File.OpenRead(image))
                        {
                            int column = sheetService.ColumnLetterToNumber(Col);

                            var picture = sheet.Drawings.AddPicture($"{i}{j}Picture", stream);


                            picture.SetSize(colWidth - 6, rowHeight - 6);

                            int YOffset = (rowHeight - (int)(picture.Size.Height / 9525)) / 2;
                            int XOffset = (colWidth - (int)(picture.Size.Width / 9525)) / 2;

                            picture.SetPosition(Row - 1, YOffset, column - 1, XOffset);
                        }

                        Col = sheetService.GetNextCombination(Col);

                        if (Col == "CA")
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 1, pagePplIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];
                            }

                            sheet.Cells[$"B{Row}"].Value = i + 1;
                            sheet.Cells[$"C{Row}"].Value = distributions[i]?.Description;
                            if (!string.IsNullOrEmpty(orderedPatDistributionComments[i].Comment))
                                sheet.Cells[$"CD{Row}"].Value = orderedPatDistributionComments[i]?.Comment;

                            pagePplIndex++;
                            Col = "I";
                        }
                    }


                    Col = "I";
                    Row++;
                    if (Row > 38)
                    {
                        sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];

                        if (sheet == null)
                        {
                            sheetService.AddOtherSheet(package, 1, pagePplIndex.ToString());
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pagePplIndex + 1}"];
                        }

                        pagePplIndex++;
                        pageOprIndex = pagePplIndex;
                        Row = 12;
                    }
                    else
                    {
                        sheet = package.Workbook.Worksheets[$"{sheetNames}{pageOprIndex}"];
                        pagePplIndex = pageOprIndex;
                    }

                }

                if (!string.IsNullOrEmpty(PAT.HistoricalAbility))
                {
                    string hCol = "CD";
                    int hRow = 42;

                    dynamic data = JsonConvert.DeserializeObject(PAT.HistoricalAbility);

                    foreach (var monthData in data)
                    {
                        foreach (var month in monthData)
                        {
                            double or_o = month.First.OR_O;
                            double or_p = month.First.OR_P;

                            if (or_o != 0)
                                sheet.Cells[$"{hCol}{hRow}"].Value = or_o / 100;
                            if (or_p != 0)
                                sheet.Cells[$"{hCol}{hRow + 1}"].Value = or_p / 100;

                            hCol = sheetService.GetNextCombination(hCol);

                            if (hCol == "CJ")
                            {
                                hCol = "CD";
                                hRow = 45;
                            }
                        }
                    }
                }

                int totalPages = package.Workbook.Worksheets.Count;

                foreach (var (item, index) in package.Workbook.Worksheets.Select((item, index) => (item, index)))
                {
                    item.Cells["CH4"].Value = $"{index + 1} de {totalPages}";
                }

                //sheetService.SetPrintingOptions(package.Workbook);

                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", AplicationMonth != 0 ? $"{months[AplicationMonth]} PAT.xlsx" : "Monthly PAT.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }

        [HttpGet("Excel/HCI/{HCIId}")]
        public async Task<IActionResult> HCIExcelExport(int HCIId)
        {
            var _HCI = await _SMProcessRepository.GetHCI(HCIId, true, true, includeTransactions: true);
            if (_HCI == null)
                return NotFound("No se encontró el HCI especificado.");

            string templateName = "DataAccess/Templates/HCI Template.xlsx";
            const string sheetNames = "HCI ";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var sheet = package.Workbook.Worksheets["HCI"];

                sheet.Cells["B3"].Value = _HCI.User?.Name ?? "";
                sheet.Cells["E3"].Value = _HCI.User?.Payroll;
                sheet.Cells["J3"].Value = _HCI.User?.BirthDate.HasValue == true
                    ? _HCI.User.BirthDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    : "";
                sheet.Cells["B4"].Value = _HCI.User?.Management ?? "";
                sheet.Cells["F4"].Value = _HCI.User?.Department?.Description ?? "";
                sheet.Cells["J4"].Value = _HCI.User?.Process ?? "";
                sheet.Cells["M4"].Value = _HCI.User?.IncomesDate.HasValue == true
                    ? _HCI.User.IncomesDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    : "";


                //var imageFile = new FileInfo("path_to_your_image.jpg");
                //var picture = sheet.Drawings.AddPicture("MyImage", imageFile);

                //picture.SetPosition(1, 0, 1, 0);

                //var cellWidth = sheet.Column(1).Width * 7.5;
                //var cellHeight = sheet.Row(1).Height;

                //picture.SetSize((int)cellWidth, (int)cellHeight);

                #region Overflow registers managment
                ExcelWorksheet annexSheet = null;
                int annexCurrentRow = 1;

                void AddToAnnex(string sectionTitle, IEnumerable<string[]> rows)
                {
                    if (annexSheet == null) annexSheet = package.Workbook.Worksheets.Add("Anexos");

                    var titleRange = annexSheet.Cells[annexCurrentRow, 1];
                    titleRange.Value = $"DETALLE DE: {sectionTitle}";
                    titleRange.Style.Font.Bold = true;
                    annexCurrentRow++;

                    foreach (var row in rows)
                    {
                        for (int i = 0; i < row.Length; i++)
                        {
                            annexSheet.Cells[annexCurrentRow, i + 1].Value = row[i];
                        }
                        annexCurrentRow++;
                    }
                    annexCurrentRow += 2;
                }
                #endregion

                if (_HCI.Transactions != null)
                {
                    string? payrol = null;
                    if (_HCI.User?.Payroll > 0)
                        payrol = _HCI.User?.Payroll.ToString();

                    if (!string.IsNullOrEmpty(payrol))
                    {
                        var courses = new List<UserCourse>();
                        try
                        {
                            var coursesResponse = await _userCoursesService.GetUserCoursesAsync(payrol);
                            if (coursesResponse != null && coursesResponse.Success && coursesResponse.Data != null)
                                courses = coursesResponse.Data.ToList();
                        }
                        catch
                        {
                            courses = new List<UserCourse>();
                        }

                        #region Courses about Manuals
                        int lastRowManuals = 18;
                        int startRowManuals = 9;
                        int maxItems = lastRowManuals - startRowManuals + 1;

                        List<UserCourse> manualCourses = courses;

                        if (manualCourses.Count > maxItems)
                        {
                            for (int i = 0; i < maxItems - 1; i++)
                            {
                                sheet.Cells[$"A{startRowManuals + i}"].Value = courses[i].Date.ToString("dd/MM/yyyy");
                                sheet.Cells[$"B{startRowManuals + i}"].Value = courses[i].Course;
                            }
                            var cellAviso = sheet.Cells[$"A{lastRowManuals}"];
                            cellAviso.Value = "--- Ver detalle en Anexos ---";
                            cellAviso.Style.Font.Italic = true;

                            AddToAnnex("CAPACITACIÓN RECIBIDA SOBRE LOS MANUALES DEL DEPTO", courses.Select(c => new[] { c.Date.ToString("dd/MM/yyyy"), c.Course }));
                        }
                        else
                        {
                            int r = startRowManuals;
                            foreach (var item in courses)
                            {
                                sheet.Cells[$"A{r}"].Value = item.Date.ToString("dd/MM/yyyy");
                                sheet.Cells[$"B{r}"].Value = item.Course;
                                r++;
                            }
                        }
                        #endregion

                        sheet = package.Workbook.Worksheets.First();

                        #region Courses about company
                        int lastRowCompanyCourses = 38;
                        int startRowCompanyCourses = 21;
                        int maxCompanyCourses = lastRowCompanyCourses - startRowCompanyCourses + 1;

                        List<UserCourse> companyCourses = courses;

                        if (companyCourses.Count > maxCompanyCourses)
                        {
                            for (int i = 0; i < maxItems - 1; i++)
                            {
                                sheet.Cells[$"A{startRowManuals + i}"].Value = courses[i].Date.ToString("dd/MM/yyyy");
                                sheet.Cells[$"B{startRowManuals + i}"].Value = courses[i].Course;
                            }
                            var cellAviso = sheet.Cells[$"A{lastRowManuals}"];
                            cellAviso.Value = "--- Ver detalle en Anexos ---";
                            cellAviso.Style.Font.Italic = true;

                            AddToAnnex("CAPACITACIÓN DENTRO Y FUERA DE LA EMPRESA", courses.Select(c => new[] { c.Date.ToString("dd/MM/yyyy"), c.Course }));
                        }
                        else
                        {
                            int r = startRowManuals;
                            foreach (var item in courses)
                            {
                                sheet.Cells[$"A{r}"].Value = item.Date.ToString("dd/MM/yyyy");
                                sheet.Cells[$"B{r}"].Value = item.Course;
                                r++;
                            }
                        }
                        #endregion
                    }

                    sheet = package.Workbook.Worksheets.First();

                    #region Degrees, License & Diplomas
                    int LastRowDLD = 52;
                    int startRowDLD = 41;
                    int maxDLDItems = LastRowDLD - startRowDLD + 1;

                    var Knowledge = _HCI.Transactions.Where(t => t.Type == 3).ToList();
                    if (Knowledge.Count > maxDLDItems)
                    {
                        for (int i = 0; i < maxDLDItems - 1; i++)
                        {
                            sheet.Cells[$"A{startRowDLD + i}"].Value = Knowledge[i].DateStart.HasValue ? Knowledge[i].DateStart.Value.ToString("dd/MM/yyyy") : "";
                            sheet.Cells[$"B{startRowDLD + i}"].Value = Knowledge[i].Description;
                        }
                        var cellAviso = sheet.Cells[$"A{LastRowDLD}"];
                        cellAviso.Value = "--- Ver detalle en Anexos ---";
                        cellAviso.Style.Font.Italic = true;

                        AddToAnnex("TITULOS, LICENCIAS Y DIPLOMAS", Knowledge.Select(c => new[] { c.DateStart.Value.ToString("dd/MM/yyyy"), c.Description }));
                    }
                    else
                    {
                        int r = startRowDLD;
                        foreach (var item in Knowledge)
                        {
                            sheet.Cells[$"A{r}"].Value = item.DateStart.HasValue ? item.DateStart.Value.ToString("dd/MM/yyyy") : "";
                            sheet.Cells[$"B{r}"].Value = item.Description;
                            r++;
                        }
                    }
                    #endregion
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.CareerPaths != null)
                {
                    #region Profesional Trajectory
                    int lastRowCP = 18;
                    int startRowCP = 9;
                    int maxCPItems = lastRowCP - startRowCP + 1;

                    if (_HCI.CareerPaths.Count() > maxCPItems)
                    {
                        var items = _HCI.CareerPaths.ToList();
                        for (int i = 0; i < maxCPItems - 1; i++)
                        {
                            sheet.Cells[$"D{startRowCP + i}"].Value = items[i].CareerPathNo;
                            sheet.Cells[$"E{startRowCP + i}"].Value = items[i].ChangeDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "";
                            sheet.Cells[$"F{startRowCP + i}"].Value = items[i].Department;
                            sheet.Cells[$"G{startRowCP + i}"].Value = items[i].Process;
                            sheet.Cells[$"J{startRowCP + i}"].Value = items[i].OperationDescription;
                        }

                        sheet.Cells[$"D{lastRowCP}"].Value = "--- Ver detalle en Anexos ---";
                        AddToAnnex("TRAYECTORIA PROFESIONAL DENTRO DE LA EMPRESA", _HCI.CareerPaths.Select(cp => new[] {
                            cp.CareerPathNo.ToString(),
                            cp.ChangeDate?.ToString("dd/MM/yyyy") ?? "",
                            cp.Department,
                            cp.Process,
                            cp.OperationDescription
                        }));
                    }
                    else
                    {
                        int r = startRowCP;
                        foreach (var item in _HCI.CareerPaths)
                        {
                            sheet.Cells[$"D{r}"].Value = item.CareerPathNo;
                            sheet.Cells[$"E{r}"].Value = item.ChangeDate?.ToString("dd/MM/yyyy") ?? "";
                            sheet.Cells[$"F{r}"].Value = item.Department;
                            sheet.Cells[$"G{r}"].Value = item.Process;
                            sheet.Cells[$"J{r}"].Value = item.OperationDescription;
                            r++;
                        }
                    }
                    #endregion
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.ILUs != null)
                {
                    #region Experience (ILUs)
                    int startRowExp = 21;
                    int lastRowExp = 52;
                    int maxExpRows = lastRowExp - startRowExp + 1;

                    var experienceList = _HCI.ILUs
                        .Where(e => e.isActive)
                        .GroupBy(e => new { e.DistributionId, ILUCategory = GetILUCategory(e.ILULevel?.ILULevelCode) })
                        .Select(g => g.OrderByDescending(e => e.AcquisitionDate).First())
                        .ToList();

                    if (experienceList.Count > maxExpRows)
                    {
                        for (int i = 0; i < maxExpRows - 1; i++)
                        {
                            var item = experienceList[i];
                            FillILURow(sheet, startRowExp + i, item);
                        }

                        var avisoCell = sheet.Cells[$"D{lastRowExp}:L{lastRowExp}"];
                        avisoCell.Merge = true;
                        avisoCell.Value = "--- Ver detalle de Experiencia en Anexos ---";
                        avisoCell.Style.Font.Italic = true;

                        AddToAnnex("EXPERIENCIA (ILUs)", experienceList.Select(e => new[] {
                            (e.AcquisitionDate?.ToString("dd/MM/yyyy") ?? "") + " - " + (e.EndDate?.ToString("dd/MM/yyyy") ?? ""),
                            e.Distribution?.Description ?? "",
                            MapILUCode(e.ILULevel?.ILULevelCode)
                        }));
                    }
                    else
                    {
                        int r = startRowExp;
                        foreach (var item in experienceList)
                        {
                            FillILURow(sheet, r, item);
                            r++;
                        }
                    }
                    #endregion
                }

                void FillILURow(ExcelWorksheet sheet, int row, ILURegister item)
                {
                    sheet.Cells[$"D{row}"].Value = (item.AcquisitionDate?.ToString("dd/MM/yyyy") ?? "") + " - " + (item.EndDate?.ToString("dd/MM/yyyy") ?? "");
                    sheet.Cells[$"F{row}"].Value = item.Distribution?.Description;
                    sheet.Cells[$"L{row}"].Value = MapILUCode(item.ILULevel?.ILULevelCode);
                }

                string MapILUCode(string code)
                {
                    return code switch
                    {
                        "ITrainee" or "I" or "ILeader" or "LTrainee" or "LTraineeLeader" => "I",
                        "L" or "LLeader" or "UTrainee" or "ULeaderTrainee" => "L",
                        "U" or "ULeader" => "U",
                        _ => ""
                    };
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.Categories != null)
                {
                    #region Categories
                    int startRowCat = 10;
                    int lastRowCat = 17;
                    int maxCatRows = lastRowCat - startRowCat + 1;
                    var categoriesList = _HCI.Categories.ToList();

                    if (categoriesList.Count > maxCatRows)
                    {
                        for (int i = 0; i < maxCatRows - 1; i++)
                        {
                            var item = categoriesList[i];
                            sheet.Cells[$"N{startRowCat + i}"].Value = item.ChosenCategory?.Description ?? "";
                            sheet.Cells[$"P{startRowCat + i}"].Value = item.Date?.ToString("dd/MM/yyyy") ?? "";
                        }

                        var avisoCell = sheet.Cells[$"N{lastRowCat}"];
                        avisoCell.Value = "--- Ver detalle en Anexos ---";
                        avisoCell.Style.Font.Italic = true;
                        sheet.Cells[$"P{lastRowCat}"].Value = "";

                        AddToAnnex("CATEGORÍAS", categoriesList.Select(c => new[] {
                            c.ChosenCategory?.Description ?? "",
                            c.Date?.ToString("dd/MM/yyyy") ?? ""
                        }));
                    }
                    else
                    {
                        int r = startRowCat;
                        foreach (var item in categoriesList)
                        {
                            sheet.Cells[$"N{r}"].Value = item.ChosenCategory?.Description ?? "";
                            sheet.Cells[$"P{r}"].Value = item.Date?.ToString("dd/MM/yyyy") ?? "";
                            r++;
                        }
                    }
                    #endregion
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.Commentaries != null)
                {
                    #region Special Notes
                    int startRowComm = 20;
                    int lastRowComm = 52;
                    int maxCommRows = lastRowComm - startRowComm + 1;
                    var commentaryList = _HCI.Commentaries.ToList();

                    if (commentaryList.Count > maxCommRows)
                    {
                        for (int i = 0; i < maxCommRows - 1; i++)
                        {
                            sheet.Cells[$"N{startRowComm + i}"].Value = commentaryList[i].Comment;
                        }

                        var avisoCell = sheet.Cells[$"N{lastRowComm}"];
                        avisoCell.Value = "--- Ver notas adicionales completas en Anexos ---";
                        avisoCell.Style.Font.Italic = true;
                        avisoCell.Style.Font.Color.SetColor(System.Drawing.Color.Gray);

                        AddToAnnex("NOTAS ESPECIALES", commentaryList.Select(c => new[] { c.Comment }));
                    }
                    else
                    {
                        int r = startRowComm;
                        foreach (var item in commentaryList)
                        {
                            sheet.Cells[$"N{r}"].Value = item.Comment;
                            r++;
                        }
                    }
                    #endregion
                }

                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"HCI {_HCI.User?.Name ?? "Unknown user"}.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }

        private string GetILUCategory(string? iluLevelCode)
        {
            return iluLevelCode switch
            {
                "ITrainee" or "I" or "ILeader" or "LTrainee" or "LTraineeLeader" => "IGroup",
                "L" or "LLeader" or "UTrainee" or "ULeaderTrainee" => "LGroup",
                "U" or "ULeader" => "UGroup",
                _ => "Other"
            };
        }
    }
}
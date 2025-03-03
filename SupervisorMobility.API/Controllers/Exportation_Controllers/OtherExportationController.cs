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

namespace SupervisorMobility.API.Controllers.Exportation_Controllers
{
    [Route("api/Exportation")]
    [ApiController]
    public class OtherExportationController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _SMProcessRepository;
        private readonly ISOS_ProcessRepository _SOSProcessRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ExportationStylesService stylesService;
        private readonly ExportationImgService imgService;
        private readonly ExportationSheetService sheetService;

        public OtherExportationController(ISupervisorMobilityRepository repository, IWebHostEnvironment env, ISOS_ProcessRepository SOSrepository)
        {
            _SMProcessRepository = repository;
            _SOSProcessRepository = SOSrepository;
            _env = env ?? throw new ArgumentNullException(nameof(env));
            stylesService = new ExportationStylesService();
            imgService = new ExportationImgService();
            sheetService = new ExportationSheetService();
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
                sheet.Cells["CA4"].Value = PAT.CreationDate;
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
                                sheet.Cells[$"{hCol}{hRow}"].Value = or_o;
                            if (or_p != 0)
                                sheet.Cells[$"{hCol}{hRow + 1}"].Value = or_p;

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
                sheet.Cells["CA4"].Value = PAT.CreationDate;
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
                        int rowIndex = distributions.FindIndex(p => p.DistributionId == reg.DistributionId);
                        matrix[rowIndex, colIndex] = reg;
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

                        sheet.Cells[$"{Col}{Row}"].Value = matrix[i, j].AcquisitionDate.Value.Day;

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
                                sheet.Cells[$"{hCol}{hRow}"].Value = or_o;
                            if (or_p != 0)
                                sheet.Cells[$"{hCol}{hRow + 1}"].Value = or_p;

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

            string templateName = "DataAccess/Templates/HCI Template.xlsx";
            const string sheetNames = "HCI ";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var sheet = package.Workbook.Worksheets["HCI"];

                //#region information table

                sheet.Cells["B3"].Value = _HCI.User?.Name;
                sheet.Cells["E3"].Value = _HCI.User?.Payroll;
                sheet.Cells["J3"].Value = _HCI.User?.BirthDate.ToString();
                sheet.Cells["B4"].Value = _HCI.User?.Management;
                sheet.Cells["F4"].Value = _HCI.User?.Department?.Description;
                sheet.Cells["J4"].Value = _HCI.User?.Process;
                sheet.Cells["M4"].Value = _HCI.User?.IncomesDate.ToString();

                //var imageFile = new FileInfo("path_to_your_image.jpg");
                //var picture = sheet.Drawings.AddPicture("MyImage", imageFile);

                //picture.SetPosition(1, 0, 1, 0);

                //var cellWidth = sheet.Column(1).Width * 7.5;
                //var cellHeight = sheet.Row(1).Height;

                //picture.SetSize((int)cellWidth, (int)cellHeight);

                if (_HCI.Transactions != null)
                {
                    int pageIndex = 1;
                    int LastRow = 18;
                    int currentRow = 9;

                    #region Manuals Training

                    var ManualTraining = _HCI.Transactions.Where(t => t.Type == 1).ToList();
                    foreach (var item in ManualTraining)
                    {
                        sheet.Cells[$"A{currentRow}"].Value = (item.DateStart?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "") + " - " + (item.DateEnd?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "");
                        sheet.Cells[$"B{currentRow}"].Value = item.Description;

                        currentRow++;
                        if (currentRow > LastRow && item != ManualTraining.Last())
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 2, pageIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];
                            }

                            pageIndex++;
                            currentRow = 9;
                        }
                    }
                    #endregion

                    sheet = package.Workbook.Worksheets.First();
                    pageIndex = 1;

                    #region Company Training
                    LastRow = 38;
                    currentRow = 21;

                    var CompanyTraining = _HCI.Transactions.Where(t => t.Type == 2).ToList();
                    foreach (var item in CompanyTraining)
                    {
                        sheet.Cells[$"A{currentRow}"].Value = (item.DateStart?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "") + " - " + (item.DateEnd?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "");
                        sheet.Cells[$"B{currentRow}"].Value = item.Description;

                        currentRow++;
                        if (currentRow > LastRow && item != CompanyTraining.Last())
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 2, pageIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];
                            }

                            pageIndex++;
                            currentRow = 21;
                        }
                    }
                    #endregion

                    sheet = package.Workbook.Worksheets.First();
                    pageIndex = 1;

                    #region Knowledge
                    LastRow = 52;
                    currentRow = 41;

                    var Knowledge = _HCI.Transactions.Where(t => t.Type == 3).ToList();
                    foreach (var item in Knowledge)
                    {
                        sheet.Cells[$"A{currentRow}"].Value = item.DateEnd?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "";
                        sheet.Cells[$"B{currentRow}"].Value = item.Description;

                        currentRow++;
                        if (currentRow > LastRow && item != Knowledge.Last())
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 2, pageIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];
                            }

                            pageIndex++;
                            currentRow = 41;
                        }
                    }
                    #endregion
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.CareerPaths != null)
                {
                    int pageIndex = 1;
                    int LastRow = 18;
                    int currentRow = 9;

                    #region Career

                    foreach (var item in _HCI.CareerPaths)
                    {
                        sheet.Cells[$"D{currentRow}"].Value = item.CareerPathNo;
                        sheet.Cells[$"E{currentRow}"].Value = item.ChangeDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "";
                        sheet.Cells[$"F{currentRow}"].Value = item.Department;
                        sheet.Cells[$"G{currentRow}"].Value = item.Process;
                        sheet.Cells[$"J{currentRow}"].Value = item.OperationDescription;

                        currentRow++;
                        if (currentRow > LastRow && item != _HCI.CareerPaths.Last())
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 2, pageIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];
                            }

                            pageIndex++;
                            currentRow = 9;
                        }
                    }
                    #endregion
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.ILUs != null)
                {
                    int pageIndex = 1;
                    int LastRow = 52;
                    int currentRow = 21;

                    #region Experience

                    foreach (var item in _HCI.ILUs.Where(e => e.isActive)
                        .GroupBy(e => new { e.DistributionId, ILUCategory = GetILUCategory(e.ILULevel?.ILULevelCode) })
                        .Select(g => g.OrderByDescending(e => e.AcquisitionDate).First())
                        .ToList())
                    {
                        sheet.Cells[$"D{currentRow}"].Value = (item.AcquisitionDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "") + " - " + (item.EndDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "");
                        sheet.Cells[$"F{currentRow}"].Value = item.Distribution?.Description;
                        string iluLevlelCode = item.ILULevel?.ILULevelCode switch
                        {
                            "ITrainee" or "I" or "ILeader" or "LTrainee" or "LTraineeLeader" => "I",
                            "L" or "LLeader" or "UTrainee" or "ULeaderTrainee" => "L",
                            "U" or "ULeader" => "U",
                            _ => ""
                        };
                        sheet.Cells[$"L{currentRow}"].Value = iluLevlelCode.ToUpperInvariant();

                        currentRow++;
                        if (currentRow > LastRow && item != _HCI.ILUs.Last())
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 2, pageIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];
                            }

                            pageIndex++;
                            currentRow = 21;
                        }
                    }
                    #endregion
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.Categories != null)
                {
                    int pageIndex = 1;
                    int LastRow = 17;
                    int currentRow = 10;

                    #region Categories

                    foreach (var item in _HCI.Categories)
                    {
                        sheet.Cells[$"N{currentRow}"].Value = item.ChosenCategory?.Description ?? "";
                        sheet.Cells[$"P{currentRow}"].Value = item.Date?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "";

                        currentRow++;
                        if (currentRow > LastRow && item != _HCI.Categories.Last())
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 2, pageIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];
                            }

                            pageIndex++;
                            currentRow = 10;
                        }
                    }
                    #endregion
                }

                sheet = package.Workbook.Worksheets.First();

                if (_HCI.Commentaries != null)
                {
                    int pageIndex = 1;
                    int LastRow = 52;
                    int currentRow = 20;

                    #region Special Notes

                    foreach (var item in _HCI.Commentaries)
                    {
                        sheet.Cells[$"N{currentRow}"].Value = item.Comment;

                        currentRow++;
                        if (currentRow > LastRow && item != _HCI.Commentaries.Last())
                        {
                            sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];

                            if (sheet == null)
                            {
                                sheetService.AddOtherSheet(package, 2, pageIndex.ToString());
                                sheet = package.Workbook.Worksheets[$"{sheetNames}{pageIndex + 1}"];
                            }

                            pageIndex++;
                            currentRow = 20;
                        }
                    }
                    #endregion
                }

                //sheetService.SetPrintingOptions(package.Workbook);

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
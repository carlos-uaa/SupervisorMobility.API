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

namespace SupervisorMobility.API.Controllers.Exportation_Controllers
{
    [Route("api/Exportation")]
    [ApiController]
    public class OtherExportationController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _SMProcessRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ExportationStylesService stylesService;
        private readonly ExportationImgService imgService;
        private readonly ExportationSheetService sheetService;

        public OtherExportationController(ISupervisorMobilityRepository repository, IWebHostEnvironment env)
        {
            _SMProcessRepository = repository;
            _env = env ?? throw new ArgumentNullException(nameof(env));
            stylesService = new ExportationStylesService();
            imgService = new ExportationImgService();
            sheetService = new ExportationSheetService();
        }

        [HttpGet("Excel/PATYearly/{PATId}")]
        public async Task<IActionResult> AnalysesExcelExport(int PATId)
        {
            var PAT = await _SMProcessRepository.GetPat(PATId);

            Dictionary<OperatorRole, string> roles = new Dictionary<OperatorRole, string>
            {
                { OperatorRole.SV, "SV" },
                { OperatorRole.Lider, "LIDER" },
                { OperatorRole.CA, "C/A" },
                { OperatorRole.NI, "NI" }
            };

            var usersOfArea = (List<User>)await _SMProcessRepository.GetAllSubordinatesAsync(PAT.Supervisor.UserId);
            usersOfArea.Insert(0, PAT.Supervisor);
            usersOfArea = usersOfArea.OrderBy(p=>p.Payroll).ToList();

            var uniqueDistributions = usersOfArea.Where(user => user.ILURegisers != null)
                .SelectMany(user => user.ILURegisers)
                .Where(ilu => ilu.Distribution != null && ilu.AcquisitionDate.HasValue && ilu.AcquisitionDate.Value.Year == PAT.AplicationYear)
                .Select(ilu => ilu.DistributionId)
                .Distinct().ToList();


            List<Distribution> distributions = new List<Distribution>();

            foreach (var user in usersOfArea) 
            {
                foreach (var reg in user.ILURegisers.Where(ilu=>ilu.AcquisitionDate.HasValue && ilu.AcquisitionDate.Value.Year == PAT.AplicationYear))
                {
                    if (reg.DistributionId.HasValue)
                    {
                        if(!distributions.Any(p=>p.DistributionId == reg.DistributionId))
                        {
                            var dist = await _SMProcessRepository.GetDistributionOnlyIdAsync(reg.DistributionId.Value);
                            distributions.Add(dist);
                        }
                    }
                }
            }

            //foreach (var dist in uniqueDistributions) 
            //{
            //    distributions.Add();
            //}

            var iluLevels = await _SMProcessRepository.GetAllILULevel();
            var levels = new Dictionary<string, (string, int)>();

            foreach (var iluLevel in iluLevels)
            {
                string value; int category;
                if (iluLevel.ILULevelCode == "ITrainee")
                    value = "";
                else
                {
                    value = iluLevel.ILULevelCode[0].ToString();
                }

                category = iluLevel.ILULevelCode.Substring(1) switch { 
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

                sheet.Cells["B4"].Value = PAT.Supervisor?.Department;
                sheet.Cells["H4"].Value = PAT.Supervisor?.Area?.Description;
                sheet.Cells["P4"].Value = PAT.Supervisor?.Group?.Description;
                sheet.Cells["X4"].Value = PAT.AplicationYear;
                sheet.Cells["AI4"].Value = PAT.Supervisor?.Name;
                sheet.Cells["AU4"].Value = PAT.SSVresponsible?.Name;
                sheet.Cells["CA4"].Value = PAT.CreationDate;
                sheet.Cells["G7"].Value = PAT.KnowledgePercentage;
                sheet.Cells["G10"].Value = "S";

                //var operators = PAT.LeadershipRecords?.Select(op => op.Operator).OrderBy(p => p.Payroll).Distinct().ToList();
                //var operations = PAT.LeadershipRecords?.Select(op => op.Distribution).Distinct().ToList();

                var operatorsOrder = usersOfArea?.Select(p => p.UserId).ToList();

                var operatorRoles = PAT.PatUserRoles?.OrderBy(p => operatorsOrder.IndexOf(p.UserId)).ToList();

                var matrix = new ILURegister[distributions.Count, usersOfArea.Count];

                foreach (var uo in usersOfArea)
                {
                    int colIndex = usersOfArea.IndexOf(uo);
                    foreach (var reg in uo.ILURegisers)
                    {
                        int rowIndex = distributions.FindIndex(p=>p.DistributionId == reg.DistributionId);
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

                    for (int j = 0; j < usersOfArea.Count; j++)
                    {
                        if (Row == 12)
                        {
                            //sheet.Cells[$"{Col}5"].Value = j + 1;
                            //sheet.Cells[$"{Col}6"].Value = matrix[i, j].Operator?.Name;
                            //sheet.Cells[$"{Col}10"].Value = matrix[i, j].Operator?.Payroll;
                            //sheet.Cells[$"{Col}42"].Value = operatorRoles?[j].Comment;
                            //sheet.Cells[$"{Col}50"].Value = roles[operatorRoles[j].Role];

                            sheet.Cells[$"{Col}5"].Value = j + 1;
                            sheet.Cells[$"{Col}6"].Value = usersOfArea[j].Name;
                            sheet.Cells[$"{Col}10"].Value = usersOfArea[j].Payroll;
                            sheet.Cells[$"{Col}42"].Value = operatorRoles?[j].Comment;
                            //if(operatorRoles != null)
                            //    sheet.Cells[$"{Col}50"].Value = roles[operatorRoles[j].Role];
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


                            picture.SetSize(colWidth-6, rowHeight - 6);

                            int YOffset = (rowHeight - (int)(picture.Size.Height/9525)) / 2;
                            int XOffset = (colWidth - (int)(picture.Size.Width/9525)) / 2;

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

                //sheetService.SetPrintingOptions(package.Workbook);

                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PAT.AplicationDate.HasValue ? $"Yearly PAT {PAT.AplicationDate.Value.Year}.xlsx" : "Yearly PAT.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }
    }
}
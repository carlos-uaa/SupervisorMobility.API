using AutoMapper;
using CoreHtmlToImage;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Utilities;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.ExportationServices;
using SupervisorMobility.API.Interfaces.SOSDistribution.SOSDistributionExcel;
using System.Drawing;
using System.Text.RegularExpressions;
using Aspose.Cells;
using Aspose.Cells.Drawing;
using System.Drawing.Drawing2D;
using SupervisorMobility.API.TestingsDtos;
using DocumentFormat.OpenXml.Drawing.Charts;
using static System.Net.Mime.MediaTypeNames;

namespace SupervisorMobility.API.Controllers.Exportation_Controllers
{
    [Route("api/Exportation")]
    [ApiController]
    public class ExportationController : ControllerBase
    {
        private readonly ISOS_ProcessRepository _AnalysisProcessRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ISOSDistributionExcelService _sosDistributionExcelService;
        private readonly ExportationStylesService stylesService;
        private readonly ExportationImgService imgService;
        private readonly ExportationSheetService sheetService;

        public ExportationController(ISOS_ProcessRepository repository, IWebHostEnvironment env, ISOSDistributionExcelService sosDistributionExcelService)
        {
            _AnalysisProcessRepository = repository;
            _env = env ?? throw new ArgumentNullException(nameof(env));
            stylesService = new ExportationStylesService();
            imgService = new ExportationImgService();
            sheetService = new ExportationSheetService();
            _sosDistributionExcelService = sosDistributionExcelService;
        }

        /*
         * Please note that if any elements of the templates are changed you need to update the cells positions in here accordingly
         */
        [HttpGet("Excel/Analyses/{AnalysisId}")]
        public async Task<IActionResult> AnalysesExcelExport(int AnalysisId)
        {
            var SosAnalysis = await _AnalysisProcessRepository.GetSOSAnalysis(AnalysisId, true, true, true, true, true, true);

            string templateName = "DataAccess/Templates/Analysis Template.xlsx";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var sheet = package.Workbook.Worksheets["Analysis A"];

                #region information table

                sheet.Cells["D4"].Value = SosAnalysis.OperationName;
                sheet.Cells["D6"].Value = SosAnalysis.InternalControlNumber;
                sheet.Cells["G6"].Value = SosAnalysis.ProcessName;
                string SecurityEq = "";
                if (SosAnalysis.SOSHub?.SafetyEquipment != null && SosAnalysis.SOSHub.SafetyEquipment.Any())
                {
                    SecurityEq = string.Join(", ", SosAnalysis.SOSHub.SafetyEquipment.Select(se => se.EquipmentName));
                }
                sheet.Cells["D7"].Value = SecurityEq;
                string Tools = "";
                if (SosAnalysis.SOSHub?.ToolsUsed != null && SosAnalysis.SOSHub.ToolsUsed.Any())
                {
                    Tools = string.Join(", ", SosAnalysis.SOSHub.ToolsUsed.Select(tu => $"{tu.Tool.ToolName} ({tu.Quantity})"));
                }
                sheet.Cells["D8"].Value = Tools;
                sheet.Cells["D9"].Value = string.Join(",",SosAnalysis.SOSHub?.AppliedModels?.Select(a => a.Description) ?? Enumerable.Empty<string>());

                sheet.Cells["D10"].Value = SosAnalysis.SOSHub.TrainingTime;

                #region revitions

                if (SosAnalysis.AnalysisLogbooks != null && SosAnalysis.AnalysisLogbooks.Any())
                {
                    //SosAnalysis.AnalysisLogbooks = SosAnalysis.AnalysisLogbooks?.OrderByDescending(p => p.NoRevision).ToList();

                    List<string> Cols = new List<string> { "K", "N", "O", "P" };

                    foreach (var (item, index) in SosAnalysis.AnalysisLogbooks.Take(4).Select((item, index) => (item, index)))
                    {
                        sheet.Cells[$"{Cols[index]}4"].Value = item == SosAnalysis.AnalysisLogbooks.First() ? "N" : item.NoRevision;
                        sheet.Cells[$"{Cols[index]}5"].Value = item.Date?.ToString("dd-MMM-yyyy").Replace(".", "");
                        sheet.Cells[$"{Cols[index]}6"].Value = item.Changes;
                        sheet.Cells[$"{Cols[index]}9"].Value = item.Approver.Name;
                        sheet.Cells[$"{Cols[index]}10"].Value = item.Reviewer.Name;
                    }

                    if (SosAnalysis.AnalysisLogbooks.Skip(4).Any())
                    {
                        double TableExcelSize = 324.6 + 580 + 160;
                        double BackupTableHeight = 50;

                        sheetService.AddSheet(package, 0);

                        sheet = package.Workbook.Worksheets["Backup"];

                        int backuprow = 3;
                        foreach (var item in SosAnalysis.AnalysisLogbooks.Skip(4))
                        {
                            stylesService.BackupRowStyle(sheet, backuprow);
                            sheet.Cells[$"A{backuprow}"].Value = item.NoRevision;
                            sheet.Cells[$"B{backuprow}"].Value = item.Date;
                            sheet.Cells[$"C{backuprow}"].Value = item.Changes;
                            sheet.Cells[$"D{backuprow}"].Value = item.Approver.Name;
                            sheet.Cells[$"E{backuprow}"].Value = item.Reviewer.Name;

                            BackupTableHeight += 30;

                            if (BackupTableHeight > TableExcelSize)
                                break;

                            backuprow++;
                        }
                        sheet = package.Workbook.Worksheets["Analysis A"];
                    }
                }

                #endregion

                #endregion

                #region analyses

                double TotalRowHeight = 0;//to be able to know when to jump to next sheet

                Dictionary<string, double> rowHeights = new Dictionary<string, double> { { "A", 0 } };//page index, total rows height
                const double ChangeHeightDefaultTemplate = 580;//Total row height from an empty template to change sheet
                const double ChangeHeightExtraTemplates = 580 + 309.6 - 75.6; //default table height + information table height in default template - heght to here the table starts

                double ChangeHeight = ChangeHeightDefaultTemplate;

                Dictionary<string, (int, int)> rowIndexes = new Dictionary<string, (int, int)> { { "A", (14, 15) } }; //page index, start index, final row index
                //int startingRow = 14;//the row where the analyses start in an empty template
                //int startingRowB = 7;

                int sheetStartRow = rowIndexes["A"].Item1;

                int rowindex = 0;//to get where the final row ended

                int indexAnalysis = 0;
                int tableIndexAnalysis = 0;
                int indexSection = 0;

                var ValuesFont = sheet.Cells["D4"].Style.Font;

                foreach (var section in SosAnalysis.SOSHub.Sections)
                {
                    indexSection++;

                    int comparator = 0;
                    if (section.Analyses.Count % 2 == 0)
                        comparator = section.Analyses.Count / 2 - 1;
                    else
                        comparator = section.Analyses.Count / 2;

                    foreach (var (analysis, index) in section.Analyses.Select((analysis, index) => (analysis, index)))
                    {
                        double analysisHeight, StepHeight, CriticalHeight;
                        indexAnalysis++;
                        rowindex = sheetStartRow + tableIndexAnalysis++;

                        string fullText = string.Empty;
                        if (analysis.CriticalPoints != null && analysis.CriticalPoints.Any())
                        {
                            foreach (var (cp, cpIndex) in analysis.CriticalPoints.Select((cp, cpIndex) => (cp, cpIndex)))
                            {
                                string indexString = $"{indexAnalysis}.{cpIndex + 1}- ";
                                string critString = $"{cp}\r\n";
                                string reasonString = $"( {analysis.Reasons[cpIndex]} )";
                                if (cp != analysis.CriticalPoints.Last())
                                {
                                    reasonString += "\r\n";
                                }

                                //sheet.Cells[$"J{rowindex}"].RichText.Add(indexString);
                                //sheet.Cells[$"J{rowindex}"].RichText.Add(critString);
                                //sheet.Cells[$"J{rowindex}"].RichText.Add(reasonString);

                                fullText += $"{indexString}{critString}{reasonString}";
                            }
                        }

                        analysisHeight = stylesService.CalculateRowHeightSimple(analysis.Text, 45);
                        StepHeight = stylesService.CalculateRowHeightSimple(section.Step, 45);
                        CriticalHeight = stylesService.CalculateRowHeightByChars(fullText, 40);

                        var rowheight = Math.Max(20, Math.Max(analysisHeight, Math.Max(StepHeight, CriticalHeight)));

                        var chHeightP = (TotalRowHeight + rowheight) * 100 / ChangeHeight;

                        if (chHeightP > 100)
                        {
                            sheet.DeleteRow(rowindex);
                            rowindex--;
                            //stylesService.ChangeLastRowStyleAnalysis(sheet, rowindex, true);

                            ChangeHeight = ChangeHeightExtraTemplates;
                            string currentChar = sheet.Name.Split(" ")[1];
                            string nextPage = sheetService.GetNextCombination(currentChar);

                            sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];

                            if (sheet == null)
                            {
                                sheetService.AddSheet(package, 1, currentChar);
                                sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];
                                sheet.Cells["B3"].Value += nextPage;
                                rowHeights.Add(nextPage, 0);
                                rowIndexes.Add(nextPage, (7, 8));
                            }

                            rowHeights[currentChar] += TotalRowHeight;
                            TotalRowHeight = 0;
                            rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowindex);

                            sheetStartRow = rowindex = rowIndexes[nextPage].Item1;
                            tableIndexAnalysis = 1;
                        }

                        //if (analysis != SosAnalysis.SOSHub.Sections.First().Analyses.First() && analysis != SosAnalysis.SOSHub.Sections.Last().Analyses.Last())
                        if (tableIndexAnalysis - 1 != 0 && analysis != SosAnalysis.SOSHub.Sections.Last().Analyses.Last())
                        {
                            sheet.InsertRow(rowindex, 1);
                            bool last = analysis == section.Analyses.Last();
                            stylesService.ApplyAnalysisStyles(sheet, rowindex, last);
                        }

                        sheet.Cells[$"B{rowindex}"].Value = indexAnalysis;

                        MatchCollection result = Regex.Matches(analysis.Text, @"(\*[^*]+\*|\s*[^*]+\s*)");
                        foreach (Match text in result)
                        {
                            if (text.Value.Contains("*"))
                            {
                                var underlined = sheet.Cells[$"C{rowindex}"].RichText.Add(text.Value);
                                underlined.UnderLine = true;
                            }
                            else
                            {
                                var underlined = sheet.Cells[$"C{rowindex}"].RichText.Add(text.Value);
                                underlined.UnderLine = false;
                            }
                        }

                        if (index == comparator)
                        {
                            sheet.Cells[$"E{rowindex}"].Value = indexSection;
                            sheet.Cells[$"F{rowindex}"].Value = section.Step;

                            if (SosAnalysis.Times != null && SosAnalysis.Times.Any())
                            {
                                var timeText = SosAnalysis.Times.FirstOrDefault(p => p.SectionId == section.SectionId)?.Time;

                                if (!string.IsNullOrEmpty(timeText))
                                {
                                    string[] times = timeText.Split('.');
                                    double minutes = 0;
                                    if (double.TryParse(times[0], out double minutesResult))
                                        minutes = minutesResult / 60;

                                    sheet.Cells[$"H{rowindex}"].Style.Numberformat.Format = "0.##";
                                    sheet.Cells[$"I{rowindex}"].Style.Numberformat.Format = "0.###";

                                    if (minutes > 0)
                                        sheet.Cells[$"H{rowindex}"].Value = minutes;
                                    if (times.Length > 1 && double.TryParse(times[1], out double secondsResult))
                                        sheet.Cells[$"I{rowindex}"].Value = secondsResult / 100;
                                }
                            }

                        }

                        sheet.Cells[$"J{rowindex}"].RichText.Add(fullText);

                        //var rowheight = Math.Max(20, Math.Max(analysisHeight,Math.Max( StepHeight, CriticalHeight)));

                        TotalRowHeight += rowheight;

                        sheet.Rows[rowindex].Height = rowheight;
                    }
                }

                const double DefaultRowH = 40;


                double templateExtrahight = 25.1 + 8 + 16.2 + 15; //Time row height + whitespace + abnormalities headers + second row in analysis headers

                string currentWorkingIndex = sheet.Name.Split(" ")[1];

                rowHeights[currentWorkingIndex] += TotalRowHeight;
                rowIndexes[currentWorkingIndex] = (rowIndexes[currentWorkingIndex].Item1, rowindex);

                ChangeHeight = currentWorkingIndex == "A" ? ChangeHeightDefaultTemplate : ChangeHeightExtraTemplates;
                if (rowHeights[currentWorkingIndex] < ChangeHeight)
                {
                    var idx = rowIndexes[currentWorkingIndex].Item2;
                    var height = rowHeights[currentWorkingIndex];

                    sheetService.GenerateAnalysisRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                    rowIndexes[currentWorkingIndex] = (rowIndexes[currentWorkingIndex].Item1, idx);
                    rowHeights[currentWorkingIndex] = height;
                }

                string prevSheetName = "";

                foreach (var worksheet in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Analysis")))
                {
                    string currentChar = worksheet.Name.Split(" ")[1];

                    rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowIndexes[currentChar].Item2 + 1);
                    rowHeights[currentChar] += templateExtrahight;

                    //string currentSheet = worksheet.Name.Split(" ")[1];
                    var cellTimeMin = worksheet.Cells[$"H{rowIndexes[currentChar].Item2}"];
                    var cellTimeCnt = worksheet.Cells[$"I{rowIndexes[currentChar].Item2}"];

                    cellTimeMin.Style.Numberformat.Format = "0.0#";
                    cellTimeCnt.Style.Numberformat.Format = "0.0##";

                    string formula = $"SUM(H{rowIndexes[currentChar].Item1}:H{rowIndexes[currentChar].Item2 - 1})";
                    string formula2 = $"SUM(I{rowIndexes[currentChar].Item1}:I{rowIndexes[currentChar].Item2 - 1})";

                    if (!string.IsNullOrEmpty(prevSheetName))
                    {
                        string prevChar = prevSheetName.Split(" ")[1];
                        var pcellm = package.Workbook.Worksheets[prevSheetName].Cells[$"H{rowIndexes[prevChar].Item2}"];
                        var pcells = package.Workbook.Worksheets[prevSheetName].Cells[$"I{rowIndexes[prevChar].Item2}"];
                        if (pcellm.Value != null && !string.IsNullOrWhiteSpace(pcellm.Text))
                            formula += $"+'{prevSheetName}'!H{rowIndexes[prevChar].Item2}";
                        if (pcells.Value != null && !string.IsNullOrWhiteSpace(pcells.Text))
                            formula2 += $"+'{prevSheetName}'!I{rowIndexes[prevChar].Item2}";
                    }

                    cellTimeMin.Formula = formula;
                    cellTimeCnt.Formula = formula2;

                    cellTimeMin.Calculate();
                    cellTimeCnt.Calculate();

                    if (cellTimeMin.Value == null || string.IsNullOrWhiteSpace(cellTimeMin.Text) || cellTimeMin.Text == "0.0")
                    {
                        cellTimeMin.Value = string.Empty;
                    }

                    if (cellTimeCnt.Value == null || string.IsNullOrWhiteSpace(cellTimeCnt.Text) || cellTimeCnt.Text == "0.0")
                    {
                        cellTimeCnt.Value = string.Empty;
                    }
                    prevSheetName = worksheet.Name;
                }

                //rowHeights = rowHeights.ToDictionary(kvp => kvp.Key, kvp => (kvp.Value + templateExtrahight));
                //rowIndexes = rowIndexes.ToDictionary(kvp => kvp.Key, kvp => (kvp.Value.Item1, kvp.Value.Item2 + 1));

                sheet = package.Workbook.Worksheets[0];
                string firstSheetIndex = sheet.Name.Split(" ")[1];

                #endregion

                #region Abnormal cases

                ChangeHeight = ChangeHeightExtraTemplates;

                rowIndexes = rowIndexes.ToDictionary(kvp => kvp.Key, kvp => (kvp.Value.Item1, kvp.Value.Item2 + 3));

                int specialTotalHeight = 0, startingAbnormalRow = 0;

                rowindex = startingAbnormalRow = rowIndexes[firstSheetIndex].Item2;

                Dictionary<string, int> abnormalStarts = rowIndexes.ToDictionary(kpv => kpv.Key, kpv => kpv.Value.Item2);

                if (SosAnalysis.SOSHub.MaterialsUsed != null && SosAnalysis.SOSHub.MaterialsUsed.Any())
                {
                    foreach (var item in SosAnalysis.SOSHub.MaterialsUsed)
                    {
                        sheet.InsertRow(rowindex, 1);
                        specialTotalHeight += 20;
                        //bool last = item == SosAnalysis.SOSHub.MaterialsUsed.Last();
                        bool last = rowindex - startingAbnormalRow >= 7;
                        stylesService.ApplySpecialCasesRowStyle(sheet, rowindex, last);

                        sheet.Cells[$"E{rowindex}"].Value = item.Material.key;
                        sheet.Cells[$"F{rowindex}"].Value = item.Material.PartName;
                        sheet.Cells[$"H{rowindex}"].Value = item.Material.PartNumber;
                        sheet.Cells[$"K{rowindex}"].Value = item.Quantity;

                        rowindex++;

                        if (rowindex - startingAbnormalRow >= 8 && item != SosAnalysis.SOSHub.MaterialsUsed.Last())
                        {
                            string currentChar = sheet.Name.Split(" ")[1];
                            string nextPage = sheetService.GetNextCombination(currentChar);

                            sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];

                            if (sheet == null)
                            {
                                sheetService.AddSheet(package, 1, currentChar);
                                sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];

                                sheet.Cells["B3"].Value += nextPage;

                                rowHeights.Add(nextPage, 0);
                                rowIndexes.Add(nextPage, (7, 8));


                                var idx = rowIndexes[nextPage].Item2;
                                var height = rowHeights[nextPage];

                                sheetService.GenerateAnalysisRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                                rowIndexes[nextPage] = (rowIndexes[nextPage].Item1, idx + 4);
                                rowHeights[nextPage] = height;

                                abnormalStarts.Add(nextPage, idx + 4);
                            }
                            rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowindex - 1);
                            rowHeights[currentChar] += specialTotalHeight;

                            specialTotalHeight = 0;

                            rowindex = startingAbnormalRow = abnormalStarts[nextPage];
                        }
                    }
                    //last page assignments
                    string lastPage = sheet.Name.Split(" ")[1];
                    rowIndexes[lastPage] = (rowIndexes[lastPage].Item1, rowindex - 1);
                    rowHeights[lastPage] += specialTotalHeight;

                    foreach (var item in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Analysis")))
                    {
                        string currentChar = item.Name.Split(" ")[1];
                        if (rowIndexes[currentChar].Item2 - abnormalStarts[currentChar] >= 8)
                            continue;
                        else
                        {
                            int i = rowIndexes[currentChar].Item2;
                            double height = rowHeights[currentChar];
                            sheetService.GenerateAbnormalRows(item, ref height, ref i, abnormalStarts[currentChar]);
                            rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, i);
                            rowHeights[currentChar] = height;
                        }
                    }
                }
                else
                {
                    foreach (var item in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Analysis")))
                    {
                        string currentChar = item.Name.Split(" ")[1];
                        int i = rowIndexes[currentChar].Item2;
                        double height = rowHeights[currentChar];
                        sheetService.GenerateAbnormalRows(item, ref height, ref i, abnormalStarts[currentChar]);
                        rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, i);
                        rowHeights[currentChar] = height;
                    }
                }

                foreach (var item in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Analysis")))
                {
                    string currentChar = item.Name.Split(" ")[1];

                    stylesService.SetAbnormalsAndImgsStyles(item, abnormalStarts[currentChar], rowIndexes[currentChar].Item2, currentChar);
                }

                sheet = package.Workbook.Worksheets.First();

                #region images and notes

                double imgCellWidthDefTmplt = sheet.Columns[13].Width + sheet.Columns[14].Width + sheet.Columns[15].Width + sheet.Columns[16].Width;

                double imgCellWidthExtTmplt = 0;
                if (package.Workbook.Worksheets.Count > 1)
                {
                    if (package.Workbook.Worksheets[1].Name.Contains("Analysis"))
                    {
                        imgCellWidthExtTmplt = package.Workbook.Worksheets[1].Columns[13].Width;
                    }
                }

                double changeHeightP = imgService.HeightToPixels(rowHeights["A"]);

                int currentSheetColumnWidth = imgService.WidthToPixels(imgCellWidthDefTmplt);

                int offsetY = 2;

                //if anything is moved in template this needs to be updated
                double globalXoffsetA = imgService.WidthToPixels(175.39) + 23/*169.39*/, globalYoffsetA = imgService.HeightToPixels(295.2) + 20/*280.2*/;
                double globalXoffsetB = imgService.WidthToPixels(175.89) + 27/*169.89*/, globalYoffsetB = imgService.HeightToPixels(60.6) + 22/*90.6*/;

                //sheet = package.Workbook.Worksheets["Analysis A"];

                if (SosAnalysis.Illustrations != null && SosAnalysis.Illustrations.Any())
                {

                    string[] imgPath = { $"uploads/SOSAnalysis/Ilustrations/", "" };

                    int _case = SosAnalysis.Illustrations.Count > 2 ? 1 : 0;

                    bool add2ndImg = false;
                    bool ASheetfirstAttemptGrowing = true;

                    double globalXoffset = globalXoffsetA, globalYoffset = globalYoffsetA;

                    int tempindex = 0;
                    int spacing = 5;
                    foreach (var image in SosAnalysis.Illustrations)
                    {
                        bool changedSheet = true;
                        imgPath[1] = image.StorageFileName;
                        int horizontalOffset = 0;
                        using (FileStream stream = System.IO.File.OpenRead($"{imgPath[0]}{imgPath[1]}"))
                        {
                            System.Drawing.Image imgObj = System.Drawing.Image.FromStream(stream);

                            int w = imgObj.Width, h = imgObj.Height;

                            switch (_case)
                            {
                                case 0:
                                    int changeableW = currentSheetColumnWidth, changeableH = (int)changeHeightP;
                                    int changeableOff = offsetY;
                                    do
                                    {
                                        if (w > changeableW)
                                        {
                                            (h, w) = imgService.GetResizeMagnitudesMaintainingAspectRatio(w, h, changeableW, true);
                                        }
                                        else if (h + changeableOff > changeableH)
                                        {
                                            double overflow = h + changeableOff - changeableH;
                                            double percent = overflow * 100 / changeableH;
                                            if (tempindex != 0 && percent > 10)
                                            {
                                                string currentChar = sheet.Name.Split(" ")[1];
                                                string nextPage = sheetService.GetNextCombination(currentChar);

                                                sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];

                                                if (sheet == null)
                                                {
                                                    sheetService.AddSheet(package, 1, currentChar);
                                                    sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];

                                                    sheet.Cells["B3"].Value += nextPage;

                                                    rowHeights.Add(nextPage, 0);
                                                    rowIndexes.Add(nextPage, (7, 8));


                                                    var idx = rowIndexes[nextPage].Item2;
                                                    var height = rowHeights[nextPage];

                                                    sheetService.GenerateAnalysisRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                                                    idx += 4;
                                                    abnormalStarts.Add(nextPage, idx);

                                                    sheetService.GenerateAbnormalRows(sheet, ref height, ref idx, abnormalStarts[nextPage], true);

                                                    rowIndexes[nextPage] = (rowIndexes[nextPage].Item1, idx);
                                                    rowHeights[nextPage] = height;

                                                    stylesService.SetAbnormalsAndImgsStyles(sheet, abnormalStarts[nextPage], rowIndexes[nextPage].Item2, nextPage);

                                                    imgCellWidthExtTmplt = sheet.Columns[13].Width;
                                                    changedSheet = false;
                                                }

                                                changeHeightP = changeableH = imgService.HeightToPixels(rowHeights[nextPage]);
                                                currentSheetColumnWidth = changeableW = imgService.WidthToPixels(imgCellWidthExtTmplt);
                                                offsetY = changeableOff = 2;

                                                globalXoffset = globalXoffsetB;
                                                globalYoffset = globalYoffsetB;

                                            }
                                            (h, w) = imgService.GetResizeMagnitudesMaintainingAspectRatio(w, h, changeableH - changeableOff, false);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    } while (true);

                                    horizontalOffset = (changeableW - w) / 2;
                                    break;
                                case 1:
                                    (w, h) = (currentSheetColumnWidth / 2 - 20, 160);
                                    if (!add2ndImg)
                                    {
                                        horizontalOffset = 5;
                                        add2ndImg = true;
                                    }
                                    else
                                    {
                                        horizontalOffset = currentSheetColumnWidth - w - 5;
                                        add2ndImg = false;
                                    }
                                    break;
                            }

                            if (offsetY + h > changeHeightP && changedSheet)
                            {
                                string currentChar = sheet.Name.Split(" ")[1];
                                string nextPage = sheetService.GetNextCombination(currentChar);

                                sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];

                                if (sheet == null)
                                {
                                    sheetService.AddSheet(package, 1, currentChar);
                                    sheet = package.Workbook.Worksheets[$"Analysis {nextPage}"];

                                    sheet.Cells["B3"].Value += nextPage;

                                    rowHeights.Add(nextPage, 0);
                                    rowIndexes.Add(nextPage, (7, 8));

                                    var idx = rowIndexes[nextPage].Item2;
                                    var height = rowHeights[nextPage];

                                    sheetService.GenerateAnalysisRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                                    idx += 4;
                                    abnormalStarts.Add(nextPage, idx);

                                    sheetService.GenerateAbnormalRows(sheet, ref height, ref idx, abnormalStarts[nextPage], true);

                                    rowIndexes[nextPage] = (rowIndexes[nextPage].Item1, idx);
                                    rowHeights[nextPage] = height;

                                    stylesService.SetAbnormalsAndImgsStyles(sheet, abnormalStarts[nextPage], rowIndexes[nextPage].Item2, nextPage);

                                    imgCellWidthExtTmplt = sheet.Columns[13].Width;
                                }

                                changeHeightP = imgService.HeightToPixels(rowHeights[nextPage]);
                                currentSheetColumnWidth = imgService.WidthToPixels(imgCellWidthExtTmplt);

                                globalXoffset = globalXoffsetB;
                                globalYoffset = globalYoffsetB;

                                offsetY = 2;

                                changedSheet = true;
                                //spacing = 10;
                            }

                            string pictureName = image.FileName.Split(".")[0];
                            var picture = sheet.Drawings.AddPicture($"{pictureName}{tempindex}.png", stream);
                            picture.SetSize(w, h);

                            picture.SetPosition((int)globalYoffset + offsetY, (int)globalXoffset + horizontalOffset);

                            if (_case == 0)
                            {
                                offsetY += h;
                            }
                            else if (!add2ndImg)
                            {
                                offsetY += h + spacing;
                            }

                            tempindex++;
                        }
                    }

                }

                if (!string.IsNullOrEmpty(SosAnalysis.SOSHub.OtherInformation))
                {
                    double width = sheet.Columns[2].Width + sheet.Columns[3].Width + sheet.Columns[4].Width;
                    int trueWidth = imgService.WidthToPixels(width);
                    var text = SosAnalysis.SOSHub.OtherInformation;
                    foreach (var (item, index) in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Analysis")).Select((item, index) => (item, index)))
                    {
                        var result = stylesService.SplitTextByRowHeight(text, trueWidth, ValuesFont.Size, maxRowHeight: 160);
                        text = result.overflowText;
                        item.Cells[$"B{startingAbnormalRow}"].Value = result.fittingLines;
                        if (string.IsNullOrEmpty(text))
                        {
                            break;
                        }
                    }
                }



                #endregion

                #endregion

                // Save to file
                //package.Workbook.Calculate();

                sheetService.SetPrintingOptions(package.Workbook);

                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", !string.IsNullOrEmpty(SosAnalysis.InternalControlNumber) ? $"{SosAnalysis.InternalControlNumber} Analysis Report.xlsx" : "Analysis Report.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }

        [HttpGet("Excel/Sequence/{SequenceId}")]
        public async Task<IActionResult> SequenceExcelExport(int SequenceId)
        {
            var SosSequence = await _AnalysisProcessRepository.GetSOSSequence(SequenceId, true, true, true, true, true, true);

            string templateName = "DataAccess/Templates/Sequence Template.xlsx";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var sheet = package.Workbook.Worksheets["Sequence A"];

                #region information table

                sheet.Cells["D6"].Value = SosSequence.OperationName;
                sheet.Cells["D8"].Value = SosSequence.InternalControlNumber;
                sheet.Cells["G8"].Value = SosSequence.ProcessName;
                string SecurityEq = "";
                if (SosSequence.SOSHub?.SafetyEquipment != null && SosSequence.SOSHub.SafetyEquipment.Any())
                {
                    SecurityEq = string.Join(", ", SosSequence.SOSHub.SafetyEquipment.Select(se => se.EquipmentName));
                }
                sheet.Cells["D9"].Value = SecurityEq;
                string Tools = "";
                if (SosSequence.SOSHub?.ToolsUsed != null && SosSequence.SOSHub.ToolsUsed.Any())
                {
                    Tools = string.Join(", ", SosSequence.SOSHub.ToolsUsed.Select(tu => $"{tu.Tool.ToolName} ({tu.Quantity})"));
                }
                sheet.Cells["D10"].Value = Tools;
                //sheet.Cells["D11"].Value = SosSequence.SOSHub.AppliedModel?.Description;
                sheet.Cells["D12"].Value = SosSequence.SOSHub.TrainingTime;

                #region revitions

                if (SosSequence.SequenceLogbooks != null && SosSequence.SequenceLogbooks.Any())
                {
                    //SosAnalysis.AnalysisLogbooks = SosAnalysis.AnalysisLogbooks?.OrderByDescending(p => p.NoRevision).ToList();

                    List<string> Cols = new List<string> { "K", "N", "O", "P" };

                    foreach (var (item, index) in SosSequence.SequenceLogbooks.Take(4).Select((item, index) => (item, index)))
                    {
                        sheet.Cells[$"{Cols[index]}6"].Value = item == SosSequence.SequenceLogbooks.First() ? "N" : item.NoRevision;
                        sheet.Cells[$"{Cols[index]}7"].Value = item.Date?.ToString("dd-MMM-yyyy").Replace(".", "");
                        sheet.Cells[$"{Cols[index]}8"].Value = item.Changes;
                        sheet.Cells[$"{Cols[index]}11"].Value = item.Approver.Name;
                        sheet.Cells[$"{Cols[index]}12"].Value = item.Reviewer.Name;
                    }

                    if (SosSequence.SequenceLogbooks.Skip(4).Any())
                    {
                        double TableExcelSize = 324.6 + 580 + 160;
                        double BackupTableHeight = 50;

                        sheetService.AddSheet(package, 0);

                        sheet = package.Workbook.Worksheets["Backup"];

                        int backuprow = 3;
                        foreach (var item in SosSequence.SequenceLogbooks.Skip(4))
                        {
                            stylesService.BackupRowStyle(sheet, backuprow);
                            sheet.Cells[$"A{backuprow}"].Value = item.NoRevision;
                            sheet.Cells[$"B{backuprow}"].Value = item.Date;
                            sheet.Cells[$"C{backuprow}"].Value = item.Changes;
                            sheet.Cells[$"D{backuprow}"].Value = item.Approver.Name;
                            sheet.Cells[$"E{backuprow}"].Value = item.Reviewer.Name;

                            BackupTableHeight += 30;

                            if (BackupTableHeight > TableExcelSize)
                                break;

                            backuprow++;
                        }
                        sheet = package.Workbook.Worksheets["Sequence A"];
                    }
                }

                #endregion

                #endregion

                #region sequence

                double TotalRowHeight = 0;//to be able to know when to jump to next sheet

                Dictionary<string, double> rowHeights = new Dictionary<string, double> { { "A", 0 } };//page index, total rows height
                const double ChangeHeightDefaultTemplate = 580;//Total row height from an empty template to change sheet
                const double ChangeHeightExtraTemplates = 580 + 340.3 - 77.4; //default table height + information table height in default template - heght to where the table starts

                double ChangeHeight = ChangeHeightDefaultTemplate;

                Dictionary<string, (int, int)> rowIndexes = new Dictionary<string, (int, int)> { { "A", (16, 17) } }; //page index, start index, final row index
                //int startingRow = 14;//the row where the analyses start in an empty template
                //int startingRowB = 7;

                int sheetStartRow = rowIndexes["A"].Item1;

                int rowindex = 0;//to get where the final row ended

                int indexSection = 0;

                var ValuesFont = sheet.Cells["D6"].Style.Font;

                foreach (var section in SosSequence.SOSHub.Sections)
                {
                    double StepHeight = 0, CriticalHeight = 0;

                    rowindex = sheetStartRow + indexSection++;

                    string fullText = string.Empty;
                    int criticalIndex = 0;

                    foreach (var (analysis, index) in section.Analyses.Select((analysis, index) => (analysis, index)))
                    {
                        if (analysis.CriticalPoints != null && analysis.CriticalPoints.Any())
                        {
                            foreach (var (cp, cpIndex) in analysis.CriticalPoints.Select((cp, cpIndex) => (cp, cpIndex)))
                            {
                                criticalIndex++;
                                string indexString = $"{criticalIndex}.- ";
                                string critString = $"{cp}\r\n";
                                string reasonString = $"( {analysis.Reasons[cpIndex]} )";
                                if (cp != section.Analyses.Last(p => p.CriticalPoints.Any()).CriticalPoints.Last())
                                {
                                    reasonString += "\r\n";
                                }

                                //sheet.Cells[$"J{rowindex}"].RichText.Add(indexString);
                                //sheet.Cells[$"J{rowindex}"].RichText.Add(critString);
                                //sheet.Cells[$"J{rowindex}"].RichText.Add(reasonString);

                                fullText += $"{indexString}{critString}{reasonString}";
                            }
                        }
                    }
                    CriticalHeight = stylesService.CalculateRowHeight(fullText, sheet.Columns[10].Width + sheet.Columns[11].Width + sheet.Columns[12].Width, sheet.Cells["J16"].Style.Font.Size);

                    StepHeight = stylesService.CalculateRowHeight(section.Step, sheet.Columns[3].Width + sheet.Columns[4].Width + sheet.Columns[5].Width + sheet.Columns[6].Width + sheet.Columns[7].Width, sheet.Cells["B16"].Style.Font.Size);
                    var rowheight = Math.Max(20, Math.Max(StepHeight, CriticalHeight));

                    var chHeightP = (TotalRowHeight + rowheight) * 100 / ChangeHeight;

                    if (chHeightP > 100)
                    {
                        sheet.DeleteRow(rowindex);
                        rowindex--;
                        //stylesService.ChangeLastRowStyleAnalysis(sheet, rowindex, true);

                        ChangeHeight = ChangeHeightExtraTemplates;
                        string currentChar = sheet.Name.Split(" ")[1];
                        string nextPage = sheetService.GetNextCombination(currentChar);

                        sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];

                        if (sheet == null)
                        {
                            sheetService.AddSheet(package, 2, currentChar);
                            sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];
                            sheet.Cells["B3"].Value += nextPage;
                            rowHeights.Add(nextPage, 0);
                            rowIndexes.Add(nextPage, (7, 8));
                        }

                        rowHeights[currentChar] += TotalRowHeight;
                        TotalRowHeight = 0;
                        rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowindex);

                        sheetStartRow = rowindex = rowIndexes[nextPage].Item1;
                        indexSection = 1;
                    }

                    if (indexSection - 1 != 0 && section != SosSequence.SOSHub.Sections.Last())
                    {
                        sheet.InsertRow(rowindex, 1);
                        bool last = section == SosSequence.SOSHub.Sections.Last();
                        stylesService.ApplySequenceStyles(sheet, rowindex, last);
                    }

                    sheet.Cells[$"B{rowindex}"].Value = indexSection;

                    sheet.Cells[$"C{rowindex}"].Value = section.Step;

                    if (SosSequence.Times != null && SosSequence.Times.Any())
                    {
                        var timeText = SosSequence.Times.FirstOrDefault(p => p.SectionId == section.SectionId).Time;

                        if (!string.IsNullOrEmpty(timeText))
                        {
                            string[] times = timeText.Split('.');
                            double minutes = 0;
                            if (double.TryParse(times[0], out double minutesResult))
                                minutes = minutesResult / 60;
                            
                            sheet.Cells[$"H{rowindex}"].Style.Numberformat.Format = "0.##";
                            sheet.Cells[$"I{rowindex}"].Style.Numberformat.Format = "0.###";

                            if (minutes > 0)
                                sheet.Cells[$"H{rowindex}"].Value = minutes;
                            if (times.Length > 1 && double.TryParse(times[1], out double secondsResult))
                                sheet.Cells[$"I{rowindex}"].Value = secondsResult / 100;
                        }
                    }

                    sheet.Cells[$"J{rowindex}"].RichText.Add(fullText);

                    TotalRowHeight += rowheight;

                    sheet.Rows[rowindex].Height = rowheight;

                }

                const double DefaultRowH = 40;


                double templateExtrahight = 26.3 + 8.3 + 17.3 + 20.3; //Time row height + whitespace + abnormalities headers + second row in analysis headers

                string currentWorkingIndex = sheet.Name.Split(" ")[1];

                rowHeights[currentWorkingIndex] += TotalRowHeight;
                rowIndexes[currentWorkingIndex] = (rowIndexes[currentWorkingIndex].Item1, rowindex);

                string prevSheetName = "";

                ChangeHeight = currentWorkingIndex == "A" ? ChangeHeightDefaultTemplate : ChangeHeightExtraTemplates;
                if (rowHeights[currentWorkingIndex] < ChangeHeight)
                {
                    var idx = rowIndexes[currentWorkingIndex].Item2;
                    var height = rowHeights[currentWorkingIndex];

                    sheetService.GenerateSequenceRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                    rowIndexes[currentWorkingIndex] = (rowIndexes[currentWorkingIndex].Item1, idx);
                    rowHeights[currentWorkingIndex] = height;
                }

                foreach (var worksheet in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Sequence")))
                {
                    string currentChar = worksheet.Name.Split(" ")[1];

                    rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowIndexes[currentChar].Item2 + 1);
                    rowHeights[currentChar] += templateExtrahight;

                    var cellTimeMin = worksheet.Cells[$"H{rowIndexes[currentChar].Item2}"];
                    var cellTimeCnt = worksheet.Cells[$"I{rowIndexes[currentChar].Item2}"];

                    string formula = $"SUM(H{rowIndexes[currentChar].Item1}:H{rowIndexes[currentChar].Item2 - 1})";
                    string formula2 = $"SUM(I{rowIndexes[currentChar].Item1}:I{rowIndexes[currentChar].Item2 - 1})";

                    if (!string.IsNullOrEmpty(prevSheetName))
                    {
                        string prevChar = prevSheetName.Split(" ")[1];
                        var pcellm = package.Workbook.Worksheets[prevSheetName].Cells[$"H{rowIndexes[prevChar].Item2}"];
                        var pcells = package.Workbook.Worksheets[prevSheetName].Cells[$"I{rowIndexes[prevChar].Item2}"];
                        if (pcellm.Value != null && !string.IsNullOrWhiteSpace(pcellm.Text))
                            formula += $"+'{prevSheetName}'!H{rowIndexes[prevChar].Item2}";
                        if (pcells.Value != null && !string.IsNullOrWhiteSpace(pcells.Text))
                            formula2 += $"+'{prevSheetName}'!I{rowIndexes[prevChar].Item2}";
                    }

                    cellTimeMin.Style.Numberformat.Format = "0.0#";
                    cellTimeCnt.Style.Numberformat.Format = "0.0##";

                    cellTimeMin.Formula = formula;
                    cellTimeCnt.Formula = formula2;

                    cellTimeMin.Calculate();
                    cellTimeCnt.Calculate();

                    if (cellTimeMin.Value == null || string.IsNullOrWhiteSpace(cellTimeMin.Text) || cellTimeMin.Text == "0.0")
                    {
                        cellTimeMin.Value = string.Empty;
                    }

                    if (cellTimeCnt.Value == null || string.IsNullOrWhiteSpace(cellTimeCnt.Text) || cellTimeCnt.Text == "0.0")
                    {
                        cellTimeCnt.Value = string.Empty;
                    }

                    prevSheetName = worksheet.Name;
                }

                sheet = package.Workbook.Worksheets[0];
                string firstSheetIndex = sheet.Name.Split(" ")[1];

                #endregion

                #region Abnormal cases

                ChangeHeight = ChangeHeightExtraTemplates;

                rowIndexes = rowIndexes.ToDictionary(kvp => kvp.Key, kvp => (kvp.Value.Item1, kvp.Value.Item2 + 3));

                int specialTotalHeight = 0, startingAbnormalRow = 0;

                rowindex = startingAbnormalRow = rowIndexes[firstSheetIndex].Item2;

                Dictionary<string, int> abnormalStarts = rowIndexes.ToDictionary(kpv => kpv.Key, kpv => kpv.Value.Item2);

                if (SosSequence.SOSHub.MaterialsUsed != null && SosSequence.SOSHub.MaterialsUsed.Any())
                {
                    foreach (var item in SosSequence.SOSHub.MaterialsUsed)
                    {
                        sheet.InsertRow(rowindex, 1);
                        specialTotalHeight += 20;
                        //bool last = item == SosAnalysis.SOSHub.MaterialsUsed.Last();
                        bool last = rowindex - startingAbnormalRow >= 7;
                        stylesService.ApplySpecialCasesRowStyle(sheet, rowindex, last);

                        sheet.Cells[$"E{rowindex}"].Value = item.Material.key;
                        sheet.Cells[$"F{rowindex}"].Value = item.Material.PartName;
                        sheet.Cells[$"I{rowindex}"].Value = item.Material.PartNumber;
                        sheet.Cells[$"L{rowindex}"].Value = item.Quantity;

                        rowindex++;

                        if (rowindex - startingAbnormalRow >= 8 && item != SosSequence.SOSHub.MaterialsUsed.Last())
                        {
                            string currentChar = sheet.Name.Split(" ")[1];
                            string nextPage = sheetService.GetNextCombination(currentChar);

                            sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];

                            if (sheet == null)
                            {
                                sheetService.AddSheet(package, 2, currentChar);
                                sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];

                                sheet.Cells["B3"].Value += nextPage;

                                rowHeights.Add(nextPage, 0);
                                rowIndexes.Add(nextPage, (7, 8));


                                var idx = rowIndexes[nextPage].Item2;
                                var height = rowHeights[nextPage];

                                sheetService.GenerateSequenceRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                                rowIndexes[nextPage] = (rowIndexes[nextPage].Item1, idx + 4);
                                rowHeights[nextPage] = height;

                                abnormalStarts.Add(nextPage, idx + 4);
                            }
                            rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowindex - 1);
                            rowHeights[currentChar] += specialTotalHeight;

                            specialTotalHeight = 0;

                            rowindex = startingAbnormalRow = abnormalStarts[nextPage];
                        }
                    }
                    //last page assignments
                    string lastPage = sheet.Name.Split(" ")[1];
                    rowIndexes[lastPage] = (rowIndexes[lastPage].Item1, rowindex - 1);
                    rowHeights[lastPage] += specialTotalHeight;

                    foreach (var item in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Sequence")))
                    {
                        string currentChar = item.Name.Split(" ")[1];
                        if (rowIndexes[currentChar].Item2 - abnormalStarts[currentChar] >= 8)
                            continue;
                        else
                        {
                            int i = rowIndexes[currentChar].Item2;
                            double height = rowHeights[currentChar];
                            sheetService.GenerateAbnormalRows(item, ref height, ref i, abnormalStarts[currentChar]);
                            rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, i);
                            rowHeights[currentChar] = height;
                        }
                    }
                }
                else
                {
                    foreach (var item in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Sequence")))
                    {
                        string currentChar = item.Name.Split(" ")[1];
                        int i = rowIndexes[currentChar].Item2;
                        double height = rowHeights[currentChar];
                        sheetService.GenerateAbnormalRows(item, ref height, ref i, abnormalStarts[currentChar]);
                        rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, i);
                        rowHeights[currentChar] = height;
                    }
                }

                foreach (var item in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Sequence")))
                {
                    string currentChar = item.Name.Split(" ")[1];

                    stylesService.SetAbnormalsAndImgsStyles(item, abnormalStarts[currentChar], rowIndexes[currentChar].Item2, currentChar);
                }

                sheet = package.Workbook.Worksheets.First();

                #region images and notes

                double imgCellWidthDefTmplt = sheet.Columns[13].Width + sheet.Columns[14].Width + sheet.Columns[15].Width + sheet.Columns[16].Width;

                double imgCellWidthExtTmplt = 0;
                if (package.Workbook.Worksheets.Count > 1)
                {
                    if (package.Workbook.Worksheets[1].Name.Contains("Sequence"))
                    {
                        imgCellWidthExtTmplt = package.Workbook.Worksheets[1].Columns[13].Width;
                    }
                }

                double changeHeightP = imgService.HeightToPixels(rowHeights["A"]);

                int currentSheetColumnWidth = imgService.WidthToPixels(imgCellWidthDefTmplt);

                int offsetY = 2;

                //if anything is moved in template this needs to be updated
                double globalXoffsetA = imgService.WidthToPixels(211.49) + 65/*169.39*/, globalYoffsetA = imgService.HeightToPixels(370.3) + 20/*280.2*/;
                double globalXoffsetB = imgService.WidthToPixels(211.49) + 27/*169.89*/, globalYoffsetB = imgService.HeightToPixels(77.4) + 22/*90.6*/;

                //sheet = package.Workbook.Worksheets["Analysis A"];

                if (SosSequence.Illustrations != null && SosSequence.Illustrations.Any())
                {

                    string[] imgPath = { $"uploads/SOSSequence/Ilustrations/", "" };

                    int _case = SosSequence.Illustrations.Count > 2 ? 1 : 0;

                    bool add2ndImg = false;

                    double globalXoffset = globalXoffsetA, globalYoffset = globalYoffsetA;

                    int tempindex = 0;
                    int spacing = 5;
                    foreach (var image in SosSequence.Illustrations)
                    {
                        bool changedSheet = true;
                        imgPath[1] = image.StorageFileName;
                        int horizontalOffset = 0;
                        using (FileStream stream = System.IO.File.OpenRead($"{imgPath[0]}{imgPath[1]}"))
                        {
                            System.Drawing.Image imgObj = System.Drawing.Image.FromStream(stream);

                            int w = imgObj.Width, h = imgObj.Height;

                            switch (_case)
                            {
                                case 0:
                                    int changeableW = currentSheetColumnWidth, changeableH = (int)changeHeightP;
                                    int changeableOff = offsetY;
                                    do
                                    {
                                        if (w > changeableW)
                                        {
                                            (h, w) = imgService.GetResizeMagnitudesMaintainingAspectRatio(w, h, changeableW, true);
                                        }
                                        else if (h + changeableOff > changeableH)
                                        {
                                            double overflow = h + changeableOff - changeableH;
                                            double percent = overflow * 100 / changeableH;
                                            if (tempindex != 0 && percent > 10)
                                            {
                                                string currentChar = sheet.Name.Split(" ")[1];
                                                string nextPage = sheetService.GetNextCombination(currentChar);

                                                sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];

                                                if (sheet == null)
                                                {
                                                    sheetService.AddSheet(package, 2, currentChar);
                                                    sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];

                                                    sheet.Cells["B3"].Value += nextPage;

                                                    rowHeights.Add(nextPage, 0);
                                                    rowIndexes.Add(nextPage, (7, 8));


                                                    var idx = rowIndexes[nextPage].Item2;
                                                    var height = rowHeights[nextPage];

                                                    sheetService.GenerateSequenceRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                                                    idx += 4;
                                                    abnormalStarts.Add(nextPage, idx);

                                                    sheetService.GenerateAbnormalRows(sheet, ref height, ref idx, abnormalStarts[nextPage], true);

                                                    rowIndexes[nextPage] = (rowIndexes[nextPage].Item1, idx);
                                                    rowHeights[nextPage] = height;

                                                    stylesService.SetAbnormalsAndImgsStyles(sheet, abnormalStarts[nextPage], rowIndexes[nextPage].Item2, nextPage);

                                                    imgCellWidthExtTmplt = sheet.Columns[13].Width;
                                                    changedSheet = false;
                                                }

                                                changeHeightP = changeableH = imgService.HeightToPixels(rowHeights[nextPage]);
                                                currentSheetColumnWidth = changeableW = imgService.WidthToPixels(imgCellWidthExtTmplt);
                                                offsetY = changeableOff = 2;

                                                globalXoffset = globalXoffsetB;
                                                globalYoffset = globalYoffsetB;

                                            }
                                            (h, w) = imgService.GetResizeMagnitudesMaintainingAspectRatio(w, h, changeableH - changeableOff, false);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    } while (true);

                                    horizontalOffset = (changeableW - w) / 2;
                                    break;
                                case 1:
                                    (w, h) = (currentSheetColumnWidth / 2 - 20, 160);
                                    if (!add2ndImg)
                                    {
                                        horizontalOffset = 5;
                                        add2ndImg = true;
                                    }
                                    else
                                    {
                                        horizontalOffset = currentSheetColumnWidth - w - 5;
                                        add2ndImg = false;
                                    }
                                    break;
                            }

                            if (offsetY + h > changeHeightP && changedSheet)
                            {
                                string currentChar = sheet.Name.Split(" ")[1];
                                string nextPage = sheetService.GetNextCombination(currentChar);

                                sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];

                                if (sheet == null)
                                {
                                    sheetService.AddSheet(package, 2, currentChar);
                                    sheet = package.Workbook.Worksheets[$"Sequence {nextPage}"];

                                    sheet.Cells["B3"].Value += nextPage;

                                    rowHeights.Add(nextPage, 0);
                                    rowIndexes.Add(nextPage, (7, 8));

                                    var idx = rowIndexes[nextPage].Item2;
                                    var height = rowHeights[nextPage];

                                    sheetService.GenerateAnalysisRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                                    idx += 4;
                                    abnormalStarts.Add(nextPage, idx);

                                    sheetService.GenerateAbnormalRows(sheet, ref height, ref idx, abnormalStarts[nextPage], true);

                                    rowIndexes[nextPage] = (rowIndexes[nextPage].Item1, idx);
                                    rowHeights[nextPage] = height;

                                    stylesService.SetAbnormalsAndImgsStyles(sheet, abnormalStarts[nextPage], rowIndexes[nextPage].Item2, nextPage);

                                    imgCellWidthExtTmplt = sheet.Columns[13].Width;
                                }

                                changeHeightP = imgService.HeightToPixels(rowHeights[nextPage]);
                                currentSheetColumnWidth = imgService.WidthToPixels(imgCellWidthExtTmplt);

                                globalXoffset = globalXoffsetB;
                                globalYoffset = globalYoffsetB;

                                offsetY = 2;

                                changedSheet = true;
                                //spacing = 10;
                            }

                            string pictureName = image.FileName.Split(".")[0];
                            var picture = sheet.Drawings.AddPicture($"{pictureName}{tempindex}.png", stream);
                            picture.SetSize(w, h);

                            picture.SetPosition((int)globalYoffset + offsetY, (int)globalXoffset + horizontalOffset);

                            if (_case == 0)
                            {
                                offsetY += h;
                            }
                            else if (!add2ndImg)
                            {
                                offsetY += h + spacing;
                            }

                            tempindex++;
                        }
                    }

                }

                if (!string.IsNullOrEmpty(SosSequence.SOSHub.OtherInformation))
                {
                    double width = sheet.Columns[2].Width + sheet.Columns[3].Width + sheet.Columns[4].Width;
                    int trueWidth = imgService.WidthToPixels(width);
                    var text = SosSequence.SOSHub.OtherInformation;
                    foreach (var (item, index) in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("Sequence")).Select((item, index) => (item, index)))
                    {
                        var result = stylesService.SplitTextByRowHeight(text, trueWidth, ValuesFont.Size, maxRowHeight: 160);
                        text = result.overflowText;
                        item.Cells[$"B{startingAbnormalRow}"].Value = result.fittingLines;
                        if (string.IsNullOrEmpty(text))
                        {
                            break;
                        }
                    }
                }



                #endregion

                #endregion

                // Save to file
                //package.Workbook.Calculate();
                sheetService.SetPrintingOptions(package.Workbook);

                sheet.Protection.IsProtected = true;
                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", !string.IsNullOrEmpty(SosSequence.InternalControlNumber) ? $"{SosSequence.InternalControlNumber} Sequence Report.xlsx" : "Sequence Report.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }


        [HttpGet("Excel/Distribution/{DistributionId}")]
        public async Task<IActionResult> DistributionExcelExport(int DistributionId)
        {
            try
            {
                var response = await _sosDistributionExcelService.ExportSOSDistributionExcel(DistributionId);
                var fileName = await _sosDistributionExcelService.GetFileName(DistributionId);
                var res = File(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.IsNullOrEmpty(fileName) ? $"{fileName} Distribution Report.xlsx" : "Distribution Report.xlsx");
                res.EnableRangeProcessing = true;
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG EXPORT ERROR: {ex.Message}");
                Console.WriteLine($"DEBUG EXPORT STACK TRACE: {ex.StackTrace}");
                throw;
            }
        }

        [HttpGet("Excel/Combination/{CombinationId}")]
        public async Task<IActionResult> AsposeExampleExcelExport(int CombinationId)
        {
            //Aspose.Cells.License license = new Aspose.Cells.License();
            //license.SetLicense("AsposeLicense/Aspose.PDF.NET.lic");
            // Cargar plantilla
            var workbook = new Aspose.Cells.Workbook("DataAccess/Templates/Combination Template.xlsx");
            var sheet = workbook.Worksheets[0];
            //metodo que dibuja el diagrama de lineas
            var sosCombination = await _AnalysisProcessRepository.GetSOSCombination(CombinationId, true, true, true, true, true, true);
            if(sosCombination == null)
            {
                return NotFound("Combination not found.");
            }
            else
            {
                //metodo para llenar el template
                FillTemplate(workbook, sosCombination);

                //obtener la secuencia de operaciones y de ser asi dibujar el diagrama
                var operationSecuence = sosCombination.SOSCombinationOperationSequence?.OrderBy(so => so.SequenceId).ToList();

                if (operationSecuence != null && operationSecuence.Count > 0)
                {
                    FillLineDiagram(workbook, operationSecuence);

                }
                else
                {
                    FillLineDiagram(workbook, null);
                }

                var worksheet = workbook.Worksheets[0];
                //colocar la img
                //img de diagrama
                if (sosCombination.Illustrations != null && sosCombination.Illustrations.Count > 0)
                {
                    var fileid = sosCombination.Illustrations.First().FileUploadId;
                    var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

                    if (FileInfo is not null)
                    {
                        var path = System.IO.Path.Combine(_env.ContentRootPath, "uploads\\SOSCombination\\Ilustrations", FileInfo.StorageFileName);                        
                        byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(path);
                        var imageStream = new MemoryStream(imageBytes);

                // Cargar la imagen desde el archivo
                //var image = Image.FromFile(path);

                // Insertar la imagen en el Excel
                int pictureIndex = worksheet.Pictures.Add(16, 8, imageStream);
                Aspose.Cells.Drawing.Picture picture = worksheet.Pictures[pictureIndex];
                picture.Width = 612;  // en píxeles
                picture.Height = 380;


                    }
                }
            }


            // Guardar en memoria
            using var stream = new MemoryStream();
            workbook.Save(stream, Aspose.Cells.SaveFormat.Xlsx);
            stream.Position = 0;

            // Retornar como archivo descargable
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{sosCombination.ProcessName}.xlsx");
        }


        [HttpPost("Excel/Flow/{FlowId}")]
        public async Task<IActionResult> FlowExcelExport(int FlowId, List<IFormFile> Diagrams)
        {
            try
            {
                var SosFlow = await _AnalysisProcessRepository.GetSOSFlow(FlowId, includePeople: true, includeLogbooks: true, includeSOS: true);

                string templateName = "DataAccess/Templates/Flow Template.xlsx";
                MemoryStream ms = new MemoryStream();

                using var templateStream = System.IO.File.OpenRead(templateName);

                using (var package = new ExcelPackage(templateStream))
                {
                    package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                    var sheet = package.Workbook.Worksheets.First();

                    #region information table

                    sheet.Cells["C5"].Value = SosFlow.SOSHub.Plant?.Description;
                    sheet.Cells["G6"].Value = SosFlow.SOSHub.Area?.Description;
                    sheet.Cells["L6"].Value = SosFlow.SOSHub.Department?.Description;
                    sheet.Cells["R6"].Value = SosFlow.InternalControlNumber;

                    sheet.Cells["A9"].Value = SosFlow.OperationName;
                    if(SosFlow.SOSHub.ApproverOwners != null && SosFlow.SOSHub.ApproverOwners.Any())
                        sheet.Cells["A12"].Value = SosFlow.SOSHub.ApproverOwners?.First().Name;
                    if (SosFlow.SOSHub.ReviewerEditors != null && SosFlow.SOSHub.ReviewerEditors.Any())
                        sheet.Cells["D12"].Value = SosFlow.SOSHub.ReviewerEditors?.First()?.Name;
                    sheet.Cells["G13"].Value = SosFlow.ReviewerHS?.Name;
                    sheet.Cells["J12"].Value = SosFlow.Approver?.Name;

                    sheet.Cells["F15"].Value = SosFlow.CreatedAt?.ToString("dd-MMM-yyyy").Replace(".", "");
                    sheet.Cells["J15"].Value = SosFlow.TargetTime;

                    #region revitions

                    if (SosFlow.FlowLogbooks != null && SosFlow.FlowLogbooks.Any())
                    {
                        //SosAnalysis.AnalysisLogbooks = SosAnalysis.AnalysisLogbooks?.OrderByDescending(p => p.NoRevision).ToList();

                        List<string> Cols = new List<string> { "K", "N", "O", "P" };

                        foreach (var (item, index) in SosFlow.FlowLogbooks.Take(8).Select((item, index) => (item, index)))
                        {
                            sheet.Cells[$"M{8 + index}"].Value = item.Approver?.Name;
                            sheet.Cells[$"P{8 + index}"].Value = index + 1;
                            sheet.Cells[$"Q{8 + index}"].Value = item.Changes;
                            sheet.Cells[$"V{8 + index}"].Value = item.Date?.ToString("dd-MMM-yyyy").Replace(".", "");
                            sheet.Cells[$"X{8 + index}"].Value = item.NoRevision;
                        }

                        if (SosFlow.FlowLogbooks.Skip(8).Any())
                        {
                            int items = 1;
                            const int availableSlots = 26;
                            sheetService.AddSheet(package, 5);

                            int backuprow = 2;

                            sheet = package.Workbook.Worksheets["Backup"];

                            foreach (var item in SosFlow.FlowLogbooks.Skip(8))
                            {
                                sheet.Cells[$"A{backuprow}"].Value = item.NoRevision;
                                sheet.Cells[$"B{backuprow}"].Value = item.Date?.ToString("dd-MMM-yyyy").Replace(".", "");
                                sheet.Cells[$"C{backuprow}"].Value = item.Changes;
                                sheet.Cells[$"D{backuprow}"].Value = item.Approver.Name;
                                sheet.Cells[$"E{backuprow}"].Value = item.Reviewer.Name;

                                items++;
                                if (items > availableSlots)
                                    break;

                                backuprow += 2;
                            }
                            sheet = package.Workbook.Worksheets.First();
                        }
                    }

                    #endregion

                    #endregion

                    int startRow = 18;
                    int startColumn = 0;
                    int rowSpan = 29;     // Number of rows to span
                    int colSpan = 23;     // Number of columns to span
                    int i = 1;

                    double cellWidth = sheet.Column(startColumn + 1).Width * 7.5;  // Width in pixels
                    double cellHeight = sheet.Row(startRow + 1).Height * 1.33;     // Height in pixels

                    bool morePagesFlag = false;

                    foreach (var diagram in Diagrams)
                    {
                        if (morePagesFlag)
                        {
                            startRow = 8;
                            startColumn = 1;
                            rowSpan = 37;     // Number of rows to span
                            colSpan = 25;     // Number of columns to span

                            string currentChar = sheet.Name.Split(" ")[1];
                            string nextPage = sheetService.GetNextCombination(currentChar);

                            string pageName = $"Flow {nextPage}";

                            sheet = package.Workbook.Worksheets[pageName];

                            if (sheet == null)
                            {
                                sheetService.AddSheet(package, 4, nextPage);
                                sheet = package.Workbook.Worksheets[pageName];
                            }

                            sheet.Cells["B6"].Value = SosFlow.SOSHub.Plant?.Description;
                            sheet.Cells["H6"].Value = SosFlow.SOSHub.Area?.Description;
                            sheet.Cells["M6"].Value = SosFlow.SOSHub.Department?.Description;
                            sheet.Cells["S6"].Value = SosFlow.InternalControlNumber;
                        }

                        using var stream = new MemoryStream();
                        await diagram.CopyToAsync(stream);
                        stream.Position = 0;

                        var picture = sheet.Drawings.AddPicture($"Image_{i}", stream);

                        // Calculate total size for uniform cells
                        double totalWidth = cellWidth * colSpan;
                        double totalHeight = cellHeight * rowSpan;

                        picture.SetPosition(startRow, 0, startColumn, 0); // Start position
                        picture.SetSize((int)totalWidth, (int)totalHeight);
                        i++;
                        morePagesFlag = true;
                    }

                    int sheetTotal = package.Workbook.Worksheets.Where(p => p.Name.Contains("Flow")).Count();

                    sheet.Cells["X6"].Value = 1;
                    sheet.Cells["Y6"].Value = sheetTotal;
                    foreach (var (item, index) in package.Workbook.Worksheets.Where(p => p.Name.Contains("Flow")).Skip(1).Select((item, index) => (item, index)))
                    {
                        item.Cells["Y6"].Value = index + 2;
                        item.Cells["Z6"].Value = sheetTotal;
                    }

                    // Save to file
                    //package.Workbook.Calculate();
                    sheetService.SetPrintingOptions(package.Workbook);

                    sheet.Protection.IsProtected = true;
                    package.SaveAs(ms);
                }

                ms.Position = 0;
                var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", !string.IsNullOrEmpty(SosFlow.InternalControlNumber) ? $"{SosFlow.InternalControlNumber} Flow Report.xlsx" : "Flow Report.xlsx");
                res.EnableRangeProcessing = true;
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in flow exportation: {ex.Message}\n Inner Exception: {ex.InnerException}");
                return StatusCode(500, $"An error occurred while generating the Excel file. \n Error in flow exportation: {ex.Message}\n Inner Exception: {ex.InnerException}");
            }
        }


        private void FillTemplate(Aspose.Cells.Workbook workbook, SOSCombination sosCombination)
        {
            var sheet = workbook.Worksheets[0];

           

            #region information table
            //Nombre de la operacion
            sheet.Cells["B8"].Value = sosCombination.OperationName;
            //grupo/operador/supervisor 
            if (sosCombination.Turns?.Any() ?? false)
            {
                var cellNumber = 8;
                foreach (var turn in sosCombination.Turns)
                {
                    sheet.Cells[$"Z{cellNumber + 1}"].Value = turn.TurnType == null ? "" : turn.TurnType;
                    sheet.Cells[$"AH{cellNumber + 1}"].Value = turn.Operator?.Name == null ? " " : turn.Operator?.Name;
                    sheet.Cells[$"BE{cellNumber + 1}"].Value = turn.Supervisor?.Name == null ? " " : turn.Supervisor?.Name;
                    cellNumber++;
                }
            }
            //elaboro
            sheet.Cells["B13"].Value = sosCombination.SOSHub?.ApproverOwners?.FirstOrDefault()?.Name == null ? " " : sosCombination.SOSHub?.ApproverOwners?.FirstOrDefault()?.Name;
            //reviso
            sheet.Cells["F13"].Value = sosCombination.CombinationLogbooks?.FirstOrDefault()?.Reviewer?.Name == null ? " " : sosCombination.CombinationLogbooks?.FirstOrDefault()?.Reviewer?.Name;
            //reviso (H y S)
            sheet.Cells["J13"].Value = sosCombination.ReviewerHS?.Name == null ? " " : sosCombination.ReviewerHS?.Name;
            //aprobo
            sheet.Cells["O13"].Value = sosCombination.CombinationLogbooks?.FirstOrDefault()?.Approver?.Name == null ? " " : sosCombination.CombinationLogbooks?.FirstOrDefault()?.Approver.Name;
            //fecha de emision
            sheet.Cells["B15"].Value = sosCombination.CreatedAt?.ToString("dd-MMM-yyyy").Replace(".", "");
            //mes de aplicacion
            sheet.Cells["H15"].Value = sosCombination.ApplicationMonth == null ? "" : sosCombination.ApplicationMonth;
            //modelos
            string Models = "";
            if (sosCombination.SOSHub?.AppliedModels != null && sosCombination.SOSHub.AppliedModels.Any())
            {
                Models = string.Join(", ", sosCombination.SOSHub.AppliedModels.Select(am => am.Description));
            }
            sheet.Cells["L15"].Value = Models;
            //tiempo de aprendizaje
            sheet.Cells["Z15"].Value = sosCombination.SOSHub?.TrainingTime == null ? " " : $"{sosCombination.SOSHub.TrainingTime} DIAS ";
            //planta
            sheet.Cells["AP15"].Value = sosCombination.SOSHub.Plant?.Description;
            //departamento (gerencia)
            sheet.Cells["BE15"].Value = sosCombination.SOSHub.Department?.Description;

            
            //volumen de produccion por turno
            sheet.Cells["B29"].Value = sosCombination.ProductionVolumePerShift == null ? " " : sosCombination.ProductionVolumePerShift;
            //tiempo tacto
            sheet.Cells["I29"].Value = sosCombination.TackTime == null ? " " : sosCombination.TackTime;
            //numero de control
            sheet.Cells["L29"].Value = sosCombination.ControlNumber == null ? "" : sosCombination.ControlNumber;
            //parte fea
            var operationSecuence = sosCombination.SOSCombinationOperationSequence?.OrderBy(so => so.SequenceId).ToList();
            if (operationSecuence != null && operationSecuence.Count > 0)
            {
                var startRow = 39;
                foreach (var operation in operationSecuence)
                {
                    //secuencia de operacion
                    sheet.Cells[$"B{startRow}"].Value = operation.SequenceId == null ? " " : operation.SectionId;
                    //nombre de la operacion
                    sheet.Cells[$"C{startRow}"].Value = operation.ProcessName == null ? " " : operation.ProcessName;
                    //partes por ciclo
                    sheet.Cells[$"H{startRow}"].Value = operation.PartsPerCycle == null ? " " : operation.PartsPerCycle;
                    //tiempo de operacion manual
                    sheet.Cells[$"I{startRow}"].Value = operation.ManualOperationTime == null ? " " : operation.ManualOperationTime;
                    //tiempo de operacion manual con maquina en automatico
                    sheet.Cells[$"J{startRow}"].Value = operation.ManualOperationTimeWithMachineInAutomatic == null ? " " : operation.ManualOperationTimeWithMachineInAutomatic;
                    //tiempo de operacion de maquina en automatico
                    sheet.Cells[$"K{startRow}"].Value = operation.AutomaticMachineOperationTime == null ? " " : operation.AutomaticMachineOperationTime;



                    startRow++;

                }
            }

            //plan de produccion y observaciones
            sheet.Cells["C54"].Value = sosCombination.ProductionPlanAndObservations == null ? " " : sosCombination.ProductionPlanAndObservations;
            if (sosCombination.CombinationLogbooks != null && sosCombination.CombinationLogbooks.Count > 0)
            {
                var mostRecentLogs = sosCombination.CombinationLogbooks?.OrderByDescending(log => log.SOSCombinationLogbookId)
                .Take(Math.Min(3, sosCombination.CombinationLogbooks.Count))
                .OrderBy(log => log.SOSCombinationLogbookId)
                .ToList();
                var logRowStart = 58;
                foreach (var log in mostRecentLogs)
                {
                    //aprobo
                    sheet.Cells[$"AJ{logRowStart}"].Value = log.Approver?.Name == null ? " " : log.Approver?.Name;
                    //cambio
                    sheet.Cells[$"AS{logRowStart}"].Value = log.Changes == null ? " " : log.Changes;
                    //fecha
                    sheet.Cells[$"BP{logRowStart}"].Value = log.Date == null ? " " : log.Date?.ToString("dd-MMM-yyyy");
                    //reviso (rev)
                    sheet.Cells[$"BX{logRowStart}"].Value = log.Reviewer?.Name == null ? " " : log.Reviewer?.Name;

                    logRowStart++;
                }
            }

            #endregion

        }
        private async void FillLineDiagram(Aspose.Cells.Workbook workbook, List<SOSCombinationOperationSequence>? operations)
        {
            var sheet = workbook.Worksheets[0];

            // Copiar el shape "Freeform 4" y dibujar uno nuevo con las mismas propiedades
            Aspose.Cells.Drawing.Shape lineaOriginal = sheet.Shapes["Freeform 4"];
            //imagen de linea senoidal 
            var path = System.IO.Path.Combine("Assets/SenoidalLines", "imagenSenH.png");
            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(path);
            var imageStreamSenH = new MemoryStream(imageBytes);

            var path2 = System.IO.Path.Combine("Assets/SenoidalLines", "imagenSenV.png");
            byte[] imageBytes2 = await System.IO.File.ReadAllBytesAsync(path2);
            var imageStreamSenV = new MemoryStream(imageBytes2);





            // Si el shape original tiene más propiedades específicas que necesitas copiar, agrégalas aquí.

            // El nuevo shape aparecerá en la hoja de Excel en la posición indicada.


            // Obtener celdas dodne se empieza a dibujar
            var celdaInicio = sheet.Cells["N39"];
            var celdaFin = sheet.Cells["S39"];
            int offsetY = sheet.Cells.GetRowHeightPixel(celdaInicio.Row) / 2;
            int dotLineLimit = 850;
            //desplazamiento en Y (altur a la que se debe iniciar el dibujo de la linea)
            offsetY = offsetY - 4;
            //bandera que indica si se debe aplicar el plus al offsetY para la linea de los pasos.
            var isOffSetY = false;
            int filaInicio = celdaInicio.Row;
            int columnaInicio = celdaInicio.Column;
            int columnaInicioByRow = celdaInicio.Column;
            string celdaFinalLinea = "N39";
            //color de linea
            var lineColor = System.Drawing.Color.Black;

            if(operations==null || operations.Count < 0)
            {
                //info de ejemplo
                List<OperationsDto> operationsExample = new List<OperationsDto>
            {
                new OperationsDto
                {

                    ManualOperationTime=0.12,
                    ManualOperationTimeWithMachineInAutomatic=0.1,
                    AutomaticMachineOperationTime=1.7,
                    StepsToNextProcess=0.02
                },
                 new OperationsDto
                {

                    ManualOperationTime=0.14,
                    ManualOperationTimeWithMachineInAutomatic=0.1,
                    AutomaticMachineOperationTime=0.5,
                    StepsToNextProcess=0.06
                },
                  new OperationsDto
                {

                    ManualOperationTime=0.1,
                    ManualOperationTimeWithMachineInAutomatic=0.0,
                    AutomaticMachineOperationTime=0.3,
                    StepsToNextProcess=0.02
                },
                   new OperationsDto
                {

                    ManualOperationTime=0.02,
                    ManualOperationTimeWithMachineInAutomatic=0.0,
                    AutomaticMachineOperationTime=0.0,
                    StepsToNextProcess=0.02
                },
                    new OperationsDto
                {

                    ManualOperationTime=0.02,
                    ManualOperationTimeWithMachineInAutomatic=0.0,
                    AutomaticMachineOperationTime=0.0,
                    StepsToNextProcess=0.02
                },
                     new OperationsDto
                {

                    ManualOperationTime=0.1,
                    ManualOperationTimeWithMachineInAutomatic=0.04,
                    AutomaticMachineOperationTime=0.5,
                    StepsToNextProcess=0.06
                },
                     new OperationsDto
                {

                    ManualOperationTime=0.1,
                    ManualOperationTimeWithMachineInAutomatic=0.0,
                    AutomaticMachineOperationTime=0.4,
                    StepsToNextProcess=0.0
                }

            };

                //proceso de dibujado del grafico ejemplo
                foreach (var operation in operationsExample)
                {
                    //dibujar linea de tiempo de operacion manual
                    int desplazamientoEnX = (int)(((50 * operation.ManualOperationTime) / 0.1));//calculo del desplazamiento en Y
                    var line = sheet.Shapes.AddLine(filaInicio, offsetY, columnaInicio, 10, 0, desplazamientoEnX);//row inicio/offsetY/columna inicio de dibujo/desplaamientoX/alto/desplazamientoY
                    line.Line.DashStyle = MsoLineDashStyle.Solid;
                    line.Line.Weight = 2;
                    line.Line.SolidFill.Color = lineColor;


                    //saber donde termina la linea para dibujar la que sigue
                    celdaFinalLinea = Aspose.Cells.CellsHelper.CellIndexToName(line.LowerRightRow, line.LowerRightColumn - 1);

                    //con eso dibujamos la linea puntuada que representa el tiempo de operacion con maquina en automatico
                    if (operation.AutomaticMachineOperationTime > 0)
                    {
                        var totalUnitsByRow = 850;//170 celdas por fila * 10 unidades por celda
                        string startCell = $"O{sheet.Cells[celdaFinalLinea].Row + 1}";
                        //distancia de inicio de linea con respecto al total del rango
                        CellArea area = CellArea.CreateCellArea(startCell, celdaFinalLinea);
                        int totalCells = (area.EndRow - area.StartRow + 1) * (area.EndColumn - area.StartColumn + 1);

                        //calculamos la distancia total desde la celda de inicio hasta la celda final de la linea
                        desplazamientoEnX = (int)(((50 * operation.AutomaticMachineOperationTime) / 0.1));
                        int totalDistanceFromStartCell = totalCells * 10;//10 unidades por celda
                        int totalDistanceWithDotLine = totalDistanceFromStartCell + desplazamientoEnX;
                        if (totalDistanceWithDotLine > totalUnitsByRow)
                        {
                            int excedente = totalDistanceWithDotLine - totalUnitsByRow;

                            desplazamientoEnX = totalUnitsByRow - totalDistanceFromStartCell;
                            line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 0, desplazamientoEnX);
                            line.Line.DashStyle = MsoLineDashStyle.RoundDot;
                            line.Line.SolidFill.Color = lineColor;
                            line.Line.Weight = 2;
                            //dibujamos el exedente en la misma fila pero al inicio
                            line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY + 10, columnaInicioByRow, 10, 0, excedente);

                        }
                        else
                        {
                            line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 0, desplazamientoEnX);

                        }
                        line.Line.DashStyle = MsoLineDashStyle.RoundDot;
                        line.Line.SolidFill.Color = lineColor;
                        line.Line.Weight = 2;


                    }


                    //dibujamos la linea  en vertical para despues dibujar la linea de tiempo manual de la maquina en automatico
                    //linea de separacion
                    if (operation.ManualOperationTimeWithMachineInAutomatic > 0)
                    {
                        line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 10, 0);
                        line.Line.DashStyle = MsoLineDashStyle.Solid;
                        line.Line.SolidFill.Color = lineColor;
                        line.Line.Weight = 2;

                        //linea de tiempo manual de la maquina en automatico
                        desplazamientoEnX = (int)(((50 * operation.ManualOperationTimeWithMachineInAutomatic) / 0.1));
                        line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY + 12, sheet.Cells[celdaFinalLinea].Column, 10, 0, desplazamientoEnX);
                        line.Line.DashStyle = MsoLineDashStyle.Custom;  
                        line.Line.SolidFill.Color = lineColor;
                        line.Line.Weight = 2;
                        //asignamos a celdaFinalLinea la columna donde termino la linea de tiempo manual con maquina en automatico
                        celdaFinalLinea = Aspose.Cells.CellsHelper.CellIndexToName(line.LowerRightRow, line.LowerRightColumn - 1);
                        isOffSetY = true;
                    }


                    //dibujamos la linea de los pasos para la siguiente operacion.
                    if (operation.StepsToNextProcess > 0)
                    {
                        int indiceImagen = 0;
                        Aspose.Cells.Drawing.Picture imagen = null;
                        desplazamientoEnX = (int)(((50 * operation.StepsToNextProcess) / 0.1));
                        Aspose.Cells.Drawing.Shape rectangleForm = null;
                        if (isOffSetY)
                        {
                            //line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY + 12, sheet.Cells[celdaFinalLinea].Column, 10, 21, desplazamientoEnX);
                            rectangleForm = sheet.Shapes.AddShape(lineaOriginal.MsoDrawingType, sheet.Cells[celdaFinalLinea].Row, offsetY + 12, sheet.Cells[celdaFinalLinea].Column, 10, 21, desplazamientoEnX);
                            int fila = rectangleForm.UpperLeftRow;
                            int columna = rectangleForm.UpperLeftColumn + 1;
                            int ancho = rectangleForm.Width;
                            int alto = rectangleForm.Height;

                            if (ancho < alto)
                            {
                                indiceImagen = sheet.Pictures.Add(fila,columna, imageStreamSenV);
                                imagen = sheet.Pictures[indiceImagen];

                                imagen.Width = ancho;
                                imagen.Height = alto;
                                imagen.Top = offsetY + 12;

                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = 360-(90-anguloDiagonal);

                            }
                            else
                            {
                                indiceImagen = sheet.Pictures.Add(fila, columna, imageStreamSenH);
                                imagen = sheet.Pictures[indiceImagen];

                                // Ajustar tamaño para que coincida con la forma


                                imagen.Width = ancho;
                                imagen.Height = alto;

                                //desplazamiento en Y 
                                imagen.Top = offsetY + 12;


                                //calcular la diagonal de la forma para la inclinacion de la img
                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = anguloDiagonal;
                            }
                            
                          

                            // Opcional: enviar forma al fondo para que la imagen quede encima
                            rectangleForm.ZOrderPosition = 0;
                            rectangleForm.Fill.FillType = FillType.None;
                            rectangleForm.IsHidden = true; // Oculta el shape en la hoja
                            imagen.ZOrderPosition = 1;



                        }
                        else
                        {
                            rectangleForm = sheet.Shapes.AddShape(lineaOriginal.MsoDrawingType,sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 35, desplazamientoEnX);
                            int fila = rectangleForm.UpperLeftRow;
                            int columna = rectangleForm.UpperLeftColumn + 1;
                            int ancho = rectangleForm.Width;
                            int alto = rectangleForm.Height;


                            if (ancho < alto)
                            {
                                indiceImagen = sheet.Pictures.Add(fila, columna, imageStreamSenV);
                                imagen = sheet.Pictures[indiceImagen];

                                imagen.Width = ancho;
                                imagen.Height = alto;
                                imagen.Top = offsetY;

                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = 360 - (90 - anguloDiagonal);

                            }
                            else
                            {
                                indiceImagen = sheet.Pictures.Add(fila, columna, imageStreamSenH);
                                imagen = sheet.Pictures[indiceImagen];

                                // Ajustar tamaño para que coincida con la forma


                                imagen.Width = ancho;
                                imagen.Height = alto;

                                //desplazamiento en Y 
                                imagen.Top = offsetY;


                                //calcular la diagonal de la forma para la inclinacion de la img
                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = anguloDiagonal;
                            }
                            // Opcional: enviar forma al fondo para que la imagen quede encima
                            rectangleForm.ZOrderPosition = 0;
                            rectangleForm.Fill.FillType = FillType.None;
                           
                           
                            
                            imagen.ZOrderPosition = 1;
                        }
                        //asignamos a filaInicio la columna donde termino la linea de pasos
                        celdaFinalLinea = Aspose.Cells.CellsHelper.CellIndexToName(rectangleForm.LowerRightRow, rectangleForm.LowerRightColumn - 1);
                    }

                    filaInicio = sheet.Cells[celdaFinalLinea].Row;
                    columnaInicio = sheet.Cells[celdaFinalLinea].Column;
                    isOffSetY = false;
                }
            }
            else
            {
                int desplazamientoEnX = 0;
                LineShape line = null;
                foreach (var operation in operations)
                {
                    //dibujar linea de tiempo de operacion manual
                    if(operation.ManualOperationTime!=null && operation.ManualOperationTime > 0)
                    {
                        desplazamientoEnX = (int)(((50 * operation.ManualOperationTime) / 0.1));//calculo del desplazamiento en Y
                        line = sheet.Shapes.AddLine(filaInicio, offsetY, columnaInicio, 10, 0, desplazamientoEnX);//row inicio/offsetY/columna inicio de dibujo/desplaamientoX/alto/desplazamientoY
                        line.Line.DashStyle = MsoLineDashStyle.Solid;
                        line.Line.Weight = 2;
                        line.Line.SolidFill.Color = lineColor;
                        //saber donde termina la linea para dibujar la que sigue
                        celdaFinalLinea = Aspose.Cells.CellsHelper.CellIndexToName(line.LowerRightRow, line.LowerRightColumn - 1);
                    }
                   


                    

                    //con eso dibujamos la linea puntuada que representa el tiempo de operacion con maquina en automatico
                    if (operation.AutomaticMachineOperationTime!=null && operation.AutomaticMachineOperationTime > 0)
                    {
                        var totalUnitsByRow = 850;//puntos maximos por renglon
                        string startCell = $"O{sheet.Cells[celdaFinalLinea].Row + 1}";
                        //distancia de inicio de linea con respecto al total del rango
                        CellArea area = CellArea.CreateCellArea(startCell, celdaFinalLinea);
                        int totalCells = (area.EndRow - area.StartRow + 1) * (area.EndColumn - area.StartColumn + 1);

                        //calculamos la distancia total desde la celda de inicio hasta la celda final de la linea
                        desplazamientoEnX = (int)(((50 * operation.AutomaticMachineOperationTime) / 0.1));
                        int totalDistanceFromStartCell = totalCells * 10;//10 unidades por celda
                        int totalDistanceWithDotLine = totalDistanceFromStartCell + desplazamientoEnX;
                        if (totalDistanceWithDotLine > totalUnitsByRow)
                        {
                            int excedente = totalDistanceWithDotLine - totalUnitsByRow;

                            desplazamientoEnX = totalUnitsByRow - totalDistanceFromStartCell;
                            line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 0, desplazamientoEnX);
                            line.Line.DashStyle = MsoLineDashStyle.RoundDot;
                            line.Line.SolidFill.Color = lineColor;
                            line.Line.Weight = 2;
                            //dibujamos el exedente en la misma fila pero al inicio
                            line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY + 10, columnaInicioByRow, 10, 0, excedente);

                        }
                        else
                        {
                            line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 0, desplazamientoEnX);

                        }
                        line.Line.DashStyle = MsoLineDashStyle.RoundDot;
                        line.Line.SolidFill.Color = lineColor;
                        line.Line.Weight = 2;


                    }


                    //dibujamos la linea  en vertical para despues dibujar la linea de tiempo manual de la maquina en automatico
                    //linea de separacion
                    if (operation.ManualOperationTimeWithMachineInAutomatic!=null && operation.ManualOperationTimeWithMachineInAutomatic > 0)
                    {
                        line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 10, 0);
                        line.Line.DashStyle = MsoLineDashStyle.Solid;
                        line.Line.SolidFill.Color = lineColor;
                        line.Line.Weight = 2;

                        //linea de tiempo manual de la maquina en automatico
                        desplazamientoEnX = (int)(((50 * operation.ManualOperationTimeWithMachineInAutomatic) / 0.1));
                        line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY + 12, sheet.Cells[celdaFinalLinea].Column, 10, 0, desplazamientoEnX);
                        line.Line.DashStyle = MsoLineDashStyle.Custom;
                        line.Line.SolidFill.Color = lineColor;
                        line.Line.Weight = 2;
                        //asignamos a celdaFinalLinea la columna donde termino la linea de tiempo manual con maquina en automatico
                        celdaFinalLinea = Aspose.Cells.CellsHelper.CellIndexToName(line.LowerRightRow, line.LowerRightColumn - 1);
                        isOffSetY = true;
                    }


                    //dibujamos la linea de los pasos para la siguiente operacion.
                    if (operation.StepsToNextProcess!=null && operation.StepsToNextProcess > 0)
                    {
                        int indiceImagen = 0;
                        Aspose.Cells.Drawing.Picture imagen = null;
                        desplazamientoEnX = (int)(((50 * operation.StepsToNextProcess) / 0.1));
                        Aspose.Cells.Drawing.Shape rectangleForm = null;
                        if (isOffSetY)
                        {
                            //line = sheet.Shapes.AddLine(sheet.Cells[celdaFinalLinea].Row, offsetY + 12, sheet.Cells[celdaFinalLinea].Column, 10, 21, desplazamientoEnX);
                            rectangleForm = sheet.Shapes.AddShape(lineaOriginal.MsoDrawingType, sheet.Cells[celdaFinalLinea].Row, offsetY + 12, sheet.Cells[celdaFinalLinea].Column, 10, 21, desplazamientoEnX);
                            int fila = rectangleForm.UpperLeftRow;
                            int columna = rectangleForm.UpperLeftColumn + 1;
                            int ancho = rectangleForm.Width;
                            int alto = rectangleForm.Height;

                            if (ancho < alto)
                            {
                                indiceImagen = sheet.Pictures.Add(fila, columna, imageStreamSenV);
                                imagen = sheet.Pictures[indiceImagen];

                                imagen.Width = ancho;
                                imagen.Height = alto;
                                imagen.Top = offsetY + 12;

                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = 360 - (90 - anguloDiagonal);

                            }
                            else
                            {
                                indiceImagen = sheet.Pictures.Add(fila, columna, imageStreamSenH);
                                imagen = sheet.Pictures[indiceImagen];

                                // Ajustar tamaño para que coincida con la forma


                                imagen.Width = ancho;
                                imagen.Height = alto;

                                //desplazamiento en Y 
                                imagen.Top = offsetY + 12;


                                //calcular la diagonal de la forma para la inclinacion de la img
                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = anguloDiagonal;
                            }



                            // Opcional: enviar forma al fondo para que la imagen quede encima
                            rectangleForm.ZOrderPosition = 0;
                            rectangleForm.Fill.FillType = FillType.None;
                            rectangleForm.IsHidden = true; // Oculta el shape en la hoja
                            imagen.ZOrderPosition = 1;



                        }
                        else
                        {
                            rectangleForm = sheet.Shapes.AddShape(lineaOriginal.MsoDrawingType, sheet.Cells[celdaFinalLinea].Row, offsetY, sheet.Cells[celdaFinalLinea].Column, 10, 35, desplazamientoEnX);
                            int fila = rectangleForm.UpperLeftRow;
                            int columna = rectangleForm.UpperLeftColumn + 1;
                            int ancho = rectangleForm.Width;
                            int alto = rectangleForm.Height;


                            if (ancho < alto)
                            {
                                indiceImagen = sheet.Pictures.Add(fila, columna, imageStreamSenV);
                                imagen = sheet.Pictures[indiceImagen];

                                imagen.Width = ancho;
                                imagen.Height = alto;
                                imagen.Top = offsetY;

                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = 360 - (90 - anguloDiagonal);

                            }
                            else
                            {
                                indiceImagen = sheet.Pictures.Add(fila, columna, imageStreamSenH);
                                imagen = sheet.Pictures[indiceImagen];

                                // Ajustar tamaño para que coincida con la forma


                                imagen.Width = ancho;
                                imagen.Height = alto;

                                //desplazamiento en Y 
                                imagen.Top = offsetY;


                                //calcular la diagonal de la forma para la inclinacion de la img
                                double anguloDiagonal = Math.Atan((double)alto / ancho) * (180 / Math.PI);
                                imagen.RotationAngle = anguloDiagonal;
                            }
                            // Opcional: enviar forma al fondo para que la imagen quede encima
                            rectangleForm.ZOrderPosition = 0;
                            rectangleForm.Fill.FillType = FillType.None;
                            rectangleForm.IsHidden = true; // Oculta el shape en la hoja
                            imagen.ZOrderPosition = 1;
                        }
                        //asignamos a filaInicio la columna donde termino la linea de pasos
                        celdaFinalLinea = Aspose.Cells.CellsHelper.CellIndexToName(rectangleForm.LowerRightRow, rectangleForm.LowerRightColumn - 1);
                    }

                    filaInicio = sheet.Cells[celdaFinalLinea].Row;
                    columnaInicio = sheet.Cells[celdaFinalLinea].Column;
                    isOffSetY = false;
                }
            }
        }
    }
}

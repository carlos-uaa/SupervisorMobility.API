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
using System.Drawing;
using System.Text.RegularExpressions;

namespace SupervisorMobility.API.Controllers.Exportation_Controllers
{
    [Route("api/Exportation")]
    [ApiController]
    public class ExportationController : ControllerBase
    {
        private readonly ISOS_ProcessRepository _AnalysisProcessRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ExportationStylesService stylesService;
        private readonly ExportationImgService imgService;
        private readonly ExportationSheetService sheetService;

        public ExportationController(ISOS_ProcessRepository repository, IWebHostEnvironment env)
        {
            _AnalysisProcessRepository = repository;
            _env = env ?? throw new ArgumentNullException(nameof(env));
            stylesService = new ExportationStylesService();
            imgService = new ExportationImgService();
            sheetService = new ExportationSheetService();
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
                            Image imgObj = Image.FromStream(stream);

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
                            Image imgObj = Image.FromStream(stream);

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
                Console.WriteLine($"DEBUG EXPORT: Starting DistributionExcelExport for ID {DistributionId}");
                var SosDistribution = await _AnalysisProcessRepository.GetSOSDistribution(DistributionId, true, true, true, true, includeTurns: true, includeTimes: true, includeCollections: true);
                Console.WriteLine($"DEBUG EXPORT: Retrieved SOSDistribution: {SosDistribution?.SOSDistributionId}");

            SOSHub Sos_Hub = await _AnalysisProcessRepository.GetSOSHub((int)SosDistribution.SOSHubId, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true);

            string templateName = "DataAccess/Templates/Distribution Template.xlsx";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var sheet = package.Workbook.Worksheets.First();

                #region information table

                sheet.Cells["B8"].Value = SosDistribution.ProcessName;
                sheet.Cells["G8"].Value = SosDistribution.InternalControlNumber;

                sheet.Cells["B10"].Value = SosDistribution.DistributionLogbooks?.First().Approver?.Name;
                sheet.Cells["E10"].Value = Sos_Hub?.ReviewerEditors?.First()?.Name;
                sheet.Cells["G10"].Value = Sos_Hub?.ApproverOwners?.First().Name;

                sheet.Cells["B12"].Value = SosDistribution.CreatedAt?.ToString("dd-MMM-yyyy").Replace(".", "");
                sheet.Cells["D12"].Value = SosDistribution.ApplicationMonth;
                //sheet.Cells["G12"].Value = SosDistribution.SOSHub.AppliedModel.Code;
                sheet.Cells["I12"].Value = SosDistribution.TackTime;
                sheet.Cells["J12"].Value = Sos_Hub?.TrainingTime;
                sheet.Cells["P12"].Value = Sos_Hub?.Plant?.Code;
                sheet.Cells["U12"].Value = Sos_Hub?.Department?.Code;

                if (SosDistribution.Turns != null && SosDistribution.Turns.Any())
                {
                    int i = 0;
                    int base_row = 8;
                    int max = Math.Min(3, SosDistribution.Turns.Count);

                    do
                    {
                        base_row += i;
                        var turn = SosDistribution.Turns.ElementAt(i);
                        sheet.Cells[$"K{base_row}"].Value = turn.TurnType;
                        sheet.Cells[$"M{base_row}"].Value = turn.Operator.Name;
                        sheet.Cells[$"V{base_row}"].Value = turn.Supervisor.Name;
                        i++;
                    } while (i < max);
                }

                Console.WriteLine($"DEBUG EXPORT: Processing application models");
                string[] models = SosDistribution.AplicationModels.Split("§", StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"DEBUG EXPORT: Models count: {models.Length}");
                if (models.Any())
                {
                    string[] cols = { "L15", "M15", "N15", "O15", "Q15" };
                    Console.WriteLine($"DEBUG EXPORT: Cols count: {cols.Length}");
                    for (int j = 0; j < Math.Min(models.Length, cols.Length); j++)
                    {
                        Console.WriteLine($"DEBUG EXPORT: Processing model {j}: '{models[j]}'");
                        if (!string.IsNullOrWhiteSpace(models[j]) && !models[j].Contains("§"))
                            sheet.Cells[$"{cols[j]}"].Value = models[j];
                    }
                }

                #endregion

                #region distribution

                Console.WriteLine($"DEBUG EXPORT: Starting distribution section");
                double TotalRowHeight = 0;//to be able to know when to jump to next sheet

                Dictionary<string, double> rowHeights = new Dictionary<string, double> { { "DISTRIBUTION", 0 } };//page index, total rows height

                const double ChangeHeightDefaultTemplate = 1068.2;//Total row height from an empty template to change sheet
                const double ChangeHeightExtraTemplates = 1308; //+ 340.3 - 77.4; //default table height + information table height in default template - heght to where the table starts

                double ChangeHeight = ChangeHeightDefaultTemplate;

                Dictionary<string, (int, int)> rowIndexes = new Dictionary<string, (int, int)> { { "DISTRIBUTION", (16, 17) } }; //page index, start index, final row index
                                                                                                                                 //int startingRow = 14;//the row where the analyses start in an empty template
                                                                                                                                 //int startingRowB = 7;

                Console.WriteLine($"DEBUG EXPORT: Created rowIndexes dictionary with key 'DISTRIBUTION'");
                int sheetStartRow = rowIndexes["DISTRIBUTION"].Item1;
                Console.WriteLine($"DEBUG EXPORT: Sheet start row: {sheetStartRow}");

                int rowindex = 0;//to get where the final row ended

                int indexSection = 0;

                var ValuesFont = sheet.Cells["B10"].Style.Font;
                ValuesFont.Size = 9;

                bool firstPage = true;

                Console.WriteLine($"DEBUG EXPORT: Processing {SosDistribution.SOSDistributionOperationSequence?.Count() ?? 0} operation sequences");
                foreach (var section in SosDistribution.SOSDistributionOperationSequence)
                {
                    Console.WriteLine($"DEBUG EXPORT: Processing section ID: {section.SOSDistributionOperationSequenceId}, SectionId: {section.SectionId}");
                    Console.WriteLine($"DEBUG EXPORT: Section is null: {section.Section == null}");
                    Console.WriteLine($"DEBUG EXPORT: Section.Analyses is null: {section.Section?.Analyses == null}");
                    Console.WriteLine($"DEBUG EXPORT: Section.Analyses count: {section.Section?.Analyses?.Count() ?? 0}");
                    
                    if (section.Section?.Analyses == null)
                    {
                        Console.WriteLine($"DEBUG EXPORT: Skipping section due to null analyses");
                        continue;
                    }
                    
                    double StepHeight = 0, CriticalHeight = 0;

                    rowindex = sheetStartRow + indexSection++;

                    string fullText = string.Empty;
                    int criticalIndex = 0;

                    foreach (var (analysis, index) in section.Section.Analyses.Select((analysis, index) => (analysis, index)))
                    {
                        if (analysis.CriticalPoints != null && analysis.CriticalPoints.Any())
                        {
                            foreach (var (cp, cpIndex) in analysis.CriticalPoints.Select((cp, cpIndex) => (cp, cpIndex)))
                            {
                                criticalIndex++;
                                string indexString = $"{criticalIndex}.- ";
                                string critString = $"{cp}\r\n";
                                string reasonString = $"( {analysis.Reasons[cpIndex]} )";
                                if (cp != section.Section.Analyses.Last(p => p.CriticalPoints.Any()).CriticalPoints.Last())
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

                    CriticalHeight = stylesService.CalculateRowHeight(fullText, sheet.Columns[8].Width + sheet.Columns[9].Width + sheet.Columns[10].Width + sheet.Columns[11].Width, ValuesFont.Size);

                    StepHeight = stylesService.CalculateRowHeight(section.Section.Step, sheet.Columns[3].Width + sheet.Columns[4].Width + sheet.Columns[5].Width + sheet.Columns[6].Width + sheet.Columns[7].Width, ValuesFont.Size);
                    var rowheight = Math.Max(21.8, Math.Max(StepHeight, CriticalHeight));

                    var chHeightP = (TotalRowHeight + rowheight) * 100 / ChangeHeight;

                    if (chHeightP > 100)
                    {
                        //Shift last row since it cant be deleted due to format design 
                        sheetService.MoveRowValuesDistribution(sheet, rowindex, firstPage);
                        rowindex--;
                        sheet.DeleteRow(rowindex);

                        ChangeHeight = ChangeHeightExtraTemplates;

                        string currentPIdx = sheet.Name.Split(" ", 2)[1];
                        int nextPage = sheetService.GetNextIndex(currentPIdx);

                        string pageName = $"HOE DISTRIBUTION ({nextPage})";
                        string nextPIdx = pageName.Split(" ", 2)[1];

                        sheet = package.Workbook.Worksheets[pageName];

                        if (sheet == null)
                        {
                            sheetService.AddSheet(package, 3, currentPIdx);
                            sheet = package.Workbook.Worksheets[pageName];
                            rowHeights.Add(nextPIdx, 0);
                            rowIndexes.Add(nextPIdx, (9, 10));
                        }

                        rowHeights[currentPIdx] += TotalRowHeight;
                        TotalRowHeight = 0;
                        rowIndexes[currentPIdx] = (rowIndexes[currentPIdx].Item1, rowindex);

                        sheetStartRow = rowindex = rowIndexes[nextPIdx].Item1;
                        indexSection = 1;
                        firstPage = false;
                    }

                    if (indexSection - 1 != 0)
                    {
                        sheet.InsertRow(rowindex, 1);
                        stylesService.ApplyDistributionStyles(sheet, rowindex, firstPage);
                    }

                    sheet.Cells[$"B{rowindex}"].Value = indexSection;

                    sheet.Cells[$"C{rowindex}"].Value = section.Section.Step;

                    if (SosDistribution.SOSDistributionOperationSequence != null && SosDistribution.SOSDistributionOperationSequence.Any())
                    {
                        var timeText = SosDistribution.SOSDistributionOperationSequence.FirstOrDefault(p => p.SectionId == section.SectionId).Times;

                        if (!string.IsNullOrEmpty(timeText) && timeText != "§§§§" && timeText != "0")
                        {
                            double[] times = Array.ConvertAll(timeText.Split("§"), s => 
                            {
                                if (string.IsNullOrEmpty(s) || s.Contains("§") || s.Contains("?") || !double.TryParse(s, out double result))
                                    return 0.0;
                                return result;
                            });

                            char col = 'K';
                            int maxIndex = Math.Min(models.Length, times.Length);
                            for (int j = 0; j < maxIndex; j++)
                            {
                                col = (char)(col + 1);

                                if (j < times.Length && times[j] == 0)
                                {
                                    //sheet.Cells[$"H{rowindex}"].Style.Numberformat.Format = "0.##";
                                    //sheet.Cells[$"I{rowindex}"].Style.Numberformat.Format = "0.###";
                                    stylesService.DistTimesNullStyle(sheet, firstPage, rowindex, ref col);
                                }
                                else if (j < times.Length)
                                {
                                    sheet.Cells[$"{col}{rowindex}"].Style.Numberformat.Format = "0.###";
                                    sheet.Cells[$"{col}{rowindex}"].Value = times[j];
                                    if (firstPage && col == 'O') col = 'P';
                                }
                            }
                        }
                    }

                    sheet.Cells[$"H{rowindex}"].RichText.Add(fullText);

                    TotalRowHeight += rowheight;

                    sheet.Rows[rowindex].Height = rowheight;

                }

                Console.WriteLine($"DEBUG EXPORT: Finished processing operation sequences, now processing sheet name");
                const double DefaultRowH = 43.6;

                Console.WriteLine($"DEBUG EXPORT: Sheet name: '{sheet.Name}'");
                var sheetNameParts = sheet.Name.Split(" ", 2);
                Console.WriteLine($"DEBUG EXPORT: Sheet name parts count: {sheetNameParts.Length}");
                
                if (sheetNameParts.Length < 2)
                {
                    Console.WriteLine($"DEBUG EXPORT: Error - sheet name doesn't have expected format with space separator");
                    throw new InvalidOperationException($"Sheet name '{sheet.Name}' doesn't have expected format");
                }
                
                string currentWorkingIndex = sheetNameParts[1];
                Console.WriteLine($"DEBUG EXPORT: Current working index: '{currentWorkingIndex}'");

                rowHeights[currentWorkingIndex] += TotalRowHeight + 21.8; // + 21.8 due to last row
                rowIndexes[currentWorkingIndex] = (rowIndexes[currentWorkingIndex].Item1, rowindex);

                string prevSheetName = "";

                bool isFirst = sheet == package.Workbook.Worksheets.First();
                ChangeHeight = isFirst ? ChangeHeightDefaultTemplate : ChangeHeightExtraTemplates;

                if (rowHeights[currentWorkingIndex] < ChangeHeight)
                {
                    var idx = rowIndexes[currentWorkingIndex].Item2;
                    var height = rowHeights[currentWorkingIndex];

                    sheetService.GenerateDistributionsRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH, isFirst);

                    rowIndexes[currentWorkingIndex] = (rowIndexes[currentWorkingIndex].Item1, idx + 1);
                    rowHeights[currentWorkingIndex] = height;
                }

                Console.WriteLine($"DEBUG EXPORT: Processing distribution worksheets");
                foreach (var worksheet in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("DISTRIBUTION")))
                {
                    Console.WriteLine($"DEBUG EXPORT: Processing worksheet: '{worksheet.Name}'");
                    var worksheetNameParts = worksheet.Name.Split(" ", 2);
                    Console.WriteLine($"DEBUG EXPORT: Worksheet name parts count: {worksheetNameParts.Length}");
                    
                    if (worksheetNameParts.Length < 2)
                    {
                        Console.WriteLine($"DEBUG EXPORT: Skipping worksheet with invalid name format: '{worksheet.Name}'");
                        continue;
                    }
                    
                    string currentChar = worksheetNameParts[1];
                    Console.WriteLine($"DEBUG EXPORT: Current char: '{currentChar}'");

                    // Ensure the key exists in the dictionary before accessing it
                    if (!rowIndexes.ContainsKey(currentChar))
                    {
                        Console.WriteLine($"DEBUG EXPORT: Skipping - key '{currentChar}' not found in rowIndexes dictionary");
                        continue;
                    }

                    rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowIndexes[currentChar].Item2 + 2);

                    Console.WriteLine($"DEBUG EXPORT: Processing AdditionalTime: '{SosDistribution.AdditionalTime}'");
                    double[] times = Array.ConvertAll(SosDistribution.AdditionalTime.Split("§"), s => 
                    {
                        if (string.IsNullOrEmpty(s) || s.Contains("§") || s.Contains("?") || !double.TryParse(s, out double result))
                            return 0.0;
                        return result;
                    });

                    Console.WriteLine($"DEBUG EXPORT: Times array length: {times.Length}");
                    if (times.Any())
                    {
                        char col = 'K';
                        for (int j = 0; j < times.Length; j++)
                        {
                            col = (char)(col + 1);
                            if (times[j] != 0)
                            {
                                sheet.Cells[$"{col}{rowIndexes[currentChar].Item2 - 1}"].Value = times[j];
                            }
                            if (isFirst && col == 'O') col = 'P';
                        }
                    }

                    char column = 'K';
                    for (int j = 0; j < models.Length; j++)
                    {
                        column = (char)(column + 1);

                        var cell = sheet.Cells[$"{column}{rowIndexes[currentChar].Item2}"];

                        worksheet.Cells[$"{column}{rowIndexes[currentChar].Item2}"].Style.Numberformat.Format = "0.0##";

                        string formula = $"SUM({column}{rowIndexes[currentChar].Item1}:{column}{rowIndexes[currentChar].Item2 - 1})";

                        if (!string.IsNullOrEmpty(prevSheetName))
                        {
                            string prevChar = prevSheetName.Split(" ", 2)[1];
                            if (rowIndexes.ContainsKey(prevChar))
                            {
                                var pcellt = package.Workbook.Worksheets[prevSheetName].Cells[$"{column}{rowIndexes[prevChar].Item2}"];
                                if (pcellt.Value != null && !string.IsNullOrWhiteSpace(pcellt.Text))
                                    formula += $"+'{prevSheetName}'!{column}{rowIndexes[prevChar].Item2}";
                            }
                        }

                        cell.Formula = formula;
                        cell.Calculate();
                        if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Text) || cell.Text == "0.0")
                        {
                            cell.Value = string.Empty;
                        }

                        if (isFirst && column == 'O') column = 'P';
                    }

                    prevSheetName = worksheet.Name;
                    stylesService.SetDistributionImgsStyles(worksheet, rowIndexes[currentChar].Item1, rowIndexes[currentChar].Item2 - 3, isFirst);
                }

                Console.WriteLine($"DEBUG EXPORT: Getting first sheet");
                sheet = package.Workbook.Worksheets[0];
                Console.WriteLine($"DEBUG EXPORT: First sheet name: '{sheet.Name}'");
                
                var firstSheetNameParts = sheet.Name.Split(" ");
                Console.WriteLine($"DEBUG EXPORT: First sheet name parts count: {firstSheetNameParts.Length}");
                
                if (firstSheetNameParts.Length < 2)
                {
                    Console.WriteLine($"DEBUG EXPORT: Error - first sheet name doesn't have expected format");
                    throw new InvalidOperationException($"First sheet name '{sheet.Name}' doesn't have expected format");
                }
                
                string firstSheetIndex = firstSheetNameParts[1];
                Console.WriteLine($"DEBUG EXPORT: First sheet index: '{firstSheetIndex}'");

                #endregion

                #region images and notes
                Console.WriteLine($"DEBUG EXPORT: Starting images and notes section");
                Console.WriteLine($"DEBUG EXPORT: Sheet columns count: {sheet.Columns?.Count() ?? 0}");
                
                // Safe column width calculation with bounds checking
                double imgCellWidthDefTmplt = 0;
                int maxColumns = sheet.Dimension?.End?.Column ?? 0;
                Console.WriteLine($"DEBUG EXPORT: Max columns in sheet: {maxColumns}");
                
                for (int colIndex = 18; colIndex <= 26; colIndex++)
                {
                    if (colIndex <= maxColumns)
                    {
                        imgCellWidthDefTmplt += sheet.Columns[colIndex].Width;
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG EXPORT: Column {colIndex} does not exist, using default width");
                        imgCellWidthDefTmplt += 8.43; // Default column width
                    }
                }
                Console.WriteLine($"DEBUG EXPORT: Calculated imgCellWidthDefTmplt: {imgCellWidthDefTmplt}");

                double imgCellWidthExtTmplt = 0;
                if (package.Workbook.Worksheets.Count > 1)
                {
                    if (package.Workbook.Worksheets[1].Name.Contains("DISTRIBUTION"))
                    {
                        Console.WriteLine($"DEBUG EXPORT: Processing second worksheet columns");
                        var secondSheet = package.Workbook.Worksheets[1];
                        Console.WriteLine($"DEBUG EXPORT: Second sheet columns count: {secondSheet.Columns?.Count() ?? 0}");
                        
                        // Safe column width calculation for second sheet
                        int maxColumnsSecond = secondSheet.Dimension?.End?.Column ?? 0;
                        Console.WriteLine($"DEBUG EXPORT: Max columns in second sheet: {maxColumnsSecond}");
                        
                        for (int colIndex = 17; colIndex <= 23; colIndex++)
                        {
                            if (colIndex <= maxColumnsSecond)
                            {
                                imgCellWidthExtTmplt += secondSheet.Columns[colIndex].Width;
                            }
                            else
                            {
                                Console.WriteLine($"DEBUG EXPORT: Second sheet column {colIndex} does not exist, using default width");
                                imgCellWidthExtTmplt += 8.43; // Default column width
                            }
                        }
                        Console.WriteLine($"DEBUG EXPORT: Calculated imgCellWidthExtTmplt: {imgCellWidthExtTmplt}");
                    }
                }

                if (!rowHeights.Any())
                {
                    throw new InvalidOperationException("rowHeights dictionary is empty - cannot calculate changeHeightP");
                }

                double changeHeightP = imgService.HeightToPixels(rowHeights.First().Value) - 40;
                int currentSheetColumnWidth = imgService.WidthToPixels(imgCellWidthDefTmplt);

                int offsetY = 2;

                //if anything is moved in template this needs to be updated
                double globalXoffsetA = imgService.WidthToPixels(105.31) + 95/*105.31*/, globalYoffsetA = imgService.HeightToPixels(230.5) + 6/*230.5*/;
                double globalXoffsetB = imgService.WidthToPixels(107.22) + 95/*107.22*/, globalYoffsetB = imgService.HeightToPixels(128.7) + 6/*90.6*/;
                var list = SosDistribution.Illustrations.ToList();
                byte[] bytes = { };

                if (SosDistribution.SOSDistributionAdditionalTime != null)
                {
                    //sheet = package.Workbook.Worksheets["Analysis A"];
                    var html = $"<table style='font-family: Arial, sans-serif; border: 1px solid black; border-collapse:collapse; width: {currentSheetColumnWidth - 10}px;'>";

                    html += $"<tr><td style='border: 1px solid black;'>TEMPO</td>";
                    for (int j = 0; j < models.Length; j++)
                    {
                        html += $"<td style='border: 1px solid black;text-align:center;'>CANTIDAD {models[j]}</td>";
                    }
                    html += $"<td style='border: 1px solid black;'>TEMPO</td>";
                    for (int j = 0; j < models.Length; j++)
                    {
                        html += $"<td style='border: 1px solid black;text-align:center;'>{models[j]}</td>";
                    }
                    html += $"</tr>";

                    string[] side = { "TOMA", "DEJA", "PASOS" };

                    Dictionary<int, double[]> quant = new Dictionary<int, double[]>{
                        { 0, Array.ConvertAll(SosDistribution.SOSDistributionAdditionalTime.TakeQuantity.Split("§",StringSplitOptions.RemoveEmptyEntries), s => double.TryParse(s, out double result) && !s.Contains("§") && !s.Contains("?") ? result : 0.0) },
                        { 1, Array.ConvertAll(SosDistribution.SOSDistributionAdditionalTime.LeaveQuantity.Split("§",StringSplitOptions.RemoveEmptyEntries), s => double.TryParse(s, out double result) && !s.Contains("§") && !s.Contains("?") ? result : 0.0) },
                        { 2, Array.ConvertAll(SosDistribution.SOSDistributionAdditionalTime.StepsQuantity.Split("§",StringSplitOptions.RemoveEmptyEntries), s => double.TryParse(s, out double result) && !s.Contains("§") && !s.Contains("?") ? result : 0.0) }
                    };
                    
                    Dictionary<int, double[]> mtime = new Dictionary<int, double[]>{
                        { 0, Array.ConvertAll(SosDistribution.SOSDistributionAdditionalTime.TakeTime.Split("§",StringSplitOptions.RemoveEmptyEntries), s => double.TryParse(s, out double result) && !s.Contains("§") && !s.Contains("?") ? result : 0.0) },
                        { 1, Array.ConvertAll(SosDistribution.SOSDistributionAdditionalTime.LeaveTime.Split("§",StringSplitOptions.RemoveEmptyEntries), s => double.TryParse(s, out double result) && !s.Contains("§") && !s.Contains("?") ? result : 0.0) },
                        { 2, Array.ConvertAll(SosDistribution.SOSDistributionAdditionalTime.StepsTime.Split("§",StringSplitOptions.RemoveEmptyEntries), s => double.TryParse(s, out double result) && !s.Contains("§") && !s.Contains("?") ? result : 0.0) }
                    };

                    for (int i = 0; i < 3; i++)
                    {

                        html += $"<tr><td style='border: 1px solid black;'>{side[i]}</td>";
                        foreach (var item in quant[i].Take(models.Length))
                        {
                            html += $"<td style='border: 1px solid black;'>{item}</td>";
                        }
                        foreach (var item in mtime[i].Take(models.Length + 1))
                        {
                            html += $"<td style='border: 1px solid black;'>{item}</td>";
                        }
                        html += $"</tr>";
                    }

                    html += $"<tr><td style='border: 1px solid black;' colspan={2 + models.Length}></td>";
                    for (int i = 1; i < models.Length + 1; i++)
                    {
                        // Check bounds before accessing arrays
                        double val0 = i < mtime[0].Length ? mtime[0][i] : 0.0;
                        double val1 = i < mtime[1].Length ? mtime[1][i] : 0.0;
                        double val2 = i < mtime[2].Length ? mtime[2][i] : 0.0;
                        
                        html += $"<td style='border: 1px solid black;'>{(val0 + val1 + val2).ToString("F2")}</td>";
                    }
                    html += $"</tr>";

                    html += "</table>";

                    var converter = new HtmlConverter();
                    bytes = converter.FromHtmlString(html, currentSheetColumnWidth - 10);

                    var temp = new FileUpload { FileName = "Case" };

                    var elementAt = SosDistribution.Illustrations != null && SosDistribution.Illustrations.Any() ? 1 : 0;

                    list.Insert(elementAt, temp);
                }

                if (list.Any())
                {

                    string[] imgPath = { $"uploads/SOSDistribution/Ilustrations/", "" };

                    double globalXoffset = globalXoffsetA, globalYoffset = globalYoffsetA;

                    int tempindex = 0;
                    int spacing = 5;
                    foreach (var image in list)
                    {
                        bool changedSheet = true;
                        imgPath[1] = image.StorageFileName;
                        int horizontalOffset = 0;
                        MemoryStream stream = new MemoryStream();
                        if (image.FileName == "Case")
                        {
                            stream = new MemoryStream(bytes);
                        }
                        else
                        {
                            FileStream fromFile = System.IO.File.OpenRead($"{imgPath[0]}{imgPath[1]}");
                            fromFile.CopyTo(stream);
                            fromFile.Close(); fromFile.Dispose();
                        }
                        Image imgObj = Image.FromStream(stream);

                        int w = imgObj.Width, h = imgObj.Height;

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
                                    string currentPIdx = sheet.Name.Split(" ", 2)[1];
                                    int nextPage = sheetService.GetNextIndex(currentPIdx);

                                    string pageName = $"HOE DISTRIBUTION ({nextPage})";
                                    string nextPIdx = pageName.Split(" ", 2)[1];

                                    sheet = package.Workbook.Worksheets[pageName];

                                    if (sheet == null)
                                    {
                                        sheetService.AddSheet(package, 3, currentPIdx);
                                        sheet = package.Workbook.Worksheets[pageName];

                                        rowHeights.Add(nextPIdx, 0);
                                        rowIndexes.Add(nextPIdx, (9, 10));


                                        var idx = rowIndexes[nextPIdx].Item2 - 1;
                                        var height = rowHeights[nextPIdx];

                                        sheetService.GenerateDistributionsRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH, false);

                                        rowIndexes[nextPIdx] = (rowIndexes[nextPIdx].Item1, idx + 3);
                                        rowHeights[nextPIdx] = height;

                                        stylesService.SetDistributionImgsStyles(sheet, rowIndexes[nextPIdx].Item1, rowIndexes[nextPIdx].Item2 - 3);

                                        imgCellWidthExtTmplt = imgCellWidthExtTmplt != 0 ? imgCellWidthExtTmplt : sheet.Columns[17].Width + sheet.Columns[18].Width + sheet.Columns[19].Width + sheet.Columns[20].Width + sheet.Columns[21].Width + sheet.Columns[22].Width + sheet.Columns[23].Width;
                                        changedSheet = false;
                                    }

                                    changeHeightP = changeableH = imgService.HeightToPixels(rowHeights[nextPIdx]);
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

                        if (offsetY + h > changeHeightP && changedSheet)
                        {
                            string currentPIdx = sheet.Name.Split(" ", 2)[1];
                            int nextPage = sheetService.GetNextIndex(currentPIdx);

                            string pageName = $"HOE DISTRIBUTION ({nextPage})";
                            string nextPIdx = pageName.Split(" ", 2)[1];

                            sheet = package.Workbook.Worksheets[pageName];

                            if (sheet == null)
                            {
                                sheetService.AddSheet(package, 3, currentPIdx);
                                sheet = package.Workbook.Worksheets[pageName];

                                rowHeights.Add(nextPIdx, 0);
                                rowIndexes.Add(nextPIdx, (9, 10));

                                var idx = rowIndexes[nextPIdx].Item2 - 1;
                                var height = rowHeights[nextPIdx];

                                sheetService.GenerateDistributionsRows(sheet, ref height, ref idx, ChangeHeight, DefaultRowH, false);

                                rowIndexes[nextPIdx] = (rowIndexes[nextPIdx].Item1, idx + 3);
                                rowHeights[nextPIdx] = height;

                                stylesService.SetDistributionImgsStyles(sheet, rowIndexes[nextPIdx].Item1, rowIndexes[nextPIdx].Item2 - 3);

                                imgCellWidthExtTmplt = imgCellWidthExtTmplt != 0 ? imgCellWidthExtTmplt : sheet.Columns[17].Width + sheet.Columns[18].Width + sheet.Columns[19].Width + sheet.Columns[20].Width + sheet.Columns[21].Width + sheet.Columns[22].Width + sheet.Columns[23].Width;
                            }

                            changeHeightP = imgService.HeightToPixels(rowHeights[nextPIdx]);
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

                        offsetY += h;

                        tempindex++;

                        stream.Close(); stream.Dispose();
                    }

                }

                if (!string.IsNullOrEmpty(SosDistribution.SOSHubs.FirstOrDefault().OtherInformation))
                {
                    double width = sheet.Columns[2].Width + sheet.Columns[3].Width + sheet.Columns[4].Width + sheet.Columns[5].Width;
                    int trueWidth = imgService.WidthToPixels(width);
                    var text = SosDistribution.SOSHubs.FirstOrDefault().OtherInformation;
                    foreach (var (item, index) in package.Workbook.Worksheets.Where(ws => ws.Name.Contains("DISTRIBUTION")).Select((item, index) => (item, index)))
                    {
                        var result = stylesService.SplitTextByRowHeight(text, trueWidth, ValuesFont.Size, maxRowHeight: 55.6, existingText: "Situación Anormal y/ o Casos Especiales");
                        text = result.overflowText;
                        item.Cells[$"B{rowIndexes.ElementAt(index).Value.Item2 - 1}"].Value = result.fittingLines;
                        if (string.IsNullOrEmpty(text))
                        {
                            break;
                        }
                    }
                }

                sheet = package.Workbook.Worksheets.First();

                var pic = sheet.Drawings["Picture 11"];
                pic.SetSize(50, 29);

                int sheetTotal = package.Workbook.Worksheets.Where(p => p.Name.Contains("DISTRIBUTION")).Count();

                sheet.Cells["X12"].Value = 1;
                sheet.Cells["Z12"].Value = sheetTotal;
                foreach (var (item, index) in package.Workbook.Worksheets.Where(p => p.Name.Contains("DISTRIBUTION")).Skip(1).Select((item, index) => (item, index)))
                {
                    item.Cells["U6"].Value = index + 2;
                    item.Cells["W6"].Value = sheetTotal;

                    var dicpic = item.Drawings["Picture 2"];
                    dicpic.SetSize(50, 29);
                }



                #endregion

                // Save to file
                //package.Workbook.Calculate();
                sheetService.SetPrintingOptions(package.Workbook);

                sheet.Protection.IsProtected = true;
                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", !string.IsNullOrEmpty(SosDistribution.InternalControlNumber) ? $"{SosDistribution.InternalControlNumber} Distribution Report.xlsx" : "Distribution Report.xlsx");
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
        public async Task<IActionResult> CombinationExcelExport(int CombinationId)
        {
            return Ok();
        }

        [HttpPost("Excel/Flow/{FlowId}")]
        public async Task<IActionResult> FlowExcelExport(int FlowId, List<IFormFile> Diagrams)
        {
            var SosFlow = await _AnalysisProcessRepository.GetSOSFlow(FlowId, includePeople:true, includeLogbooks:true, includeSOS: true);

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
                sheet.Cells["A12"].Value = SosFlow.SOSHub.ApproverOwners?.First().Name;
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
                        sheet.Cells[$"M{8+index}"].Value = item.Approver?.Name;
                        sheet.Cells[$"P{8+index}"].Value = index + 1;
                        sheet.Cells[$"Q{8 + index}"].Value = item.Changes;
                        sheet.Cells[$"V{8+index}"].Value = item.Date?.ToString("dd-MMM-yyyy").Replace(".", "");
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

                            backuprow+=2;
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

                double cellWidth = sheet.Column(startColumn+1).Width * 7.5;  // Width in pixels
                double cellHeight = sheet.Row(startRow+1).Height * 1.33;     // Height in pixels

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
    }
}

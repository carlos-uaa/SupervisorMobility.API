using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.ExportationServices;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SupervisorMobility.API.Controllers
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
         * Please note that if any elements of the template arechanged you need to update the cells positions in here accordingly
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
                    SecurityEq = string.Join(", ", SosAnalysis.SOSHub.SafetyEquipment.Select(se=>se.EquipmentName));
                }
                sheet.Cells["D7"].Value = SecurityEq;
                string Tools = "";
                if (SosAnalysis.SOSHub?.ToolsUsed != null && SosAnalysis.SOSHub.ToolsUsed.Any())
                {
                    Tools = string.Join(", ", SosAnalysis.SOSHub.ToolsUsed.Select(tu => $"{tu.Tool.ToolName} ({tu.Quantity})"));
                }
                sheet.Cells["D8"].Value = Tools;
                sheet.Cells["D9"].Value = SosAnalysis.SOSHub.AppliedModel?.Description;
                sheet.Cells["D10"].Value = SosAnalysis.SOSHub.TrainingTime;

                #region revitions

                if (SosAnalysis.AnalysisLogbooks != null && SosAnalysis.AnalysisLogbooks.Any())
                {
                    //SosAnalysis.AnalysisLogbooks = SosAnalysis.AnalysisLogbooks?.OrderByDescending(p => p.NoRevision).ToList();

                    List<string> Cols = new List<string>{ "K", "N", "O", "P" };

                    foreach (var (item, index) in SosAnalysis.AnalysisLogbooks.Take(4).Select((item, index)=>(item, index)))
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

                Dictionary<string, double> rowHeights = new Dictionary<string, double>{ { "A", 0 } };//page index, total rows height
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
                        comparator = (section.Analyses.Count / 2) - 1;
                    else
                        comparator = section.Analyses.Count / 2;

                    foreach (var (analysis, index) in section.Analyses.Select((analysis, index)=>(analysis, index)))
                    {
                        double analysisHeight, StepHeight, CriticalHeight;
                        indexAnalysis++;
                        rowindex = sheetStartRow + tableIndexAnalysis++;
                        if (analysis != SosAnalysis.SOSHub.Sections.First().Analyses.First() && analysis != SosAnalysis.SOSHub.Sections.Last().Analyses.Last())
                        {
                            sheet.InsertRow(rowindex, 1);
                            bool last = analysis == section.Analyses.Last();
                            stylesService.ApplyAnalysisStyles(sheet, rowindex, last);
                        }

                        sheet.Cells[$"B{rowindex}"].Value = indexAnalysis;

                        MatchCollection result = Regex.Matches(analysis.Text, @"(\*[^*]+\*|\s*[^*]+\s*)");
                        foreach(Match text in result)
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

                        if(index == comparator)
                        {
                            sheet.Cells[$"E{rowindex}"].Value = indexSection;
                            sheet.Cells[$"F{rowindex}"].Value = section.Step;
                            if (!string.IsNullOrEmpty(section.Time))
                            {
                                string[] times = section.Time.Split('.');
                                double minutes = double.Parse(times[0])/ 60;
                                sheet.Cells[$"H{rowindex}"].Style.Numberformat.Format = "0.##";
                                sheet.Cells[$"I{rowindex}"].Style.Numberformat.Format = "0.###";

                                sheet.Cells[$"H{rowindex}"].Value = minutes;
                                if(times.Length > 1)
                                    sheet.Cells[$"I{rowindex}"].Value = double.Parse(times[1])/100;
                            }
                        }

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

                                sheet.Cells[$"J{rowindex}"].RichText.Add(indexString);
                                sheet.Cells[$"J{rowindex}"].RichText.Add(critString);
                                sheet.Cells[$"J{rowindex}"].RichText.Add(reasonString);

                                fullText += $"{indexString}{critString}{reasonString}";
                            }
                        }

                        analysisHeight = stylesService.CalculateRowHeight(analysis.Text, sheet.Columns[3].Width + sheet.Columns[4].Width, sheet.Cells["B14"].Style.Font.Size);
                        StepHeight = stylesService.CalculateRowHeight(section.Step, (sheet.Columns[6].Width + sheet.Columns[7].Width), sheet.Cells["F14"].Style.Font.Size);
                        CriticalHeight = stylesService.CalculateRowHeight(fullText, (sheet.Columns[10].Width + sheet.Columns[11].Width + sheet.Columns[12].Width), sheet.Cells["J14"].Style.Font.Size);

                        var rowheight = Math.Max(20, Math.Max(analysisHeight,Math.Max( StepHeight, CriticalHeight)));

                        TotalRowHeight += rowheight;

                        sheet.Rows[rowindex].Height = rowheight;

                        if(TotalRowHeight >= ChangeHeight && analysis != SosAnalysis.SOSHub.Sections.Last().Analyses.Last())
                        {
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
                            tableIndexAnalysis = 0;
                        }
                    }
                }

                const double DefaultRowH = 40;
                

                double templateExtrahight = 25.1 + 8 + 16.2 + 15; //Time row height + whitespace + abnormalities headers + second row in analysis headers

                string currentWorkingIndex = sheet.Name.Split(" ")[1];

                rowHeights[currentWorkingIndex] += TotalRowHeight;
                rowIndexes[currentWorkingIndex] = (rowIndexes[currentWorkingIndex].Item1, rowindex);

                foreach (var worksheet in package.Workbook.Worksheets.Where(ws=>ws.Name.Contains("Analysis")))
                {
                    string currentChar = worksheet.Name.Split(" ")[1];
                    ChangeHeight = currentChar == "A" ? ChangeHeightDefaultTemplate : ChangeHeightExtraTemplates;
                    if (rowHeights[currentChar] < ChangeHeight)
                    {
                        var idx = rowIndexes[currentChar].Item2;
                        var height = rowHeights[currentChar];

                        sheetService.GenerateAnalysisRows(worksheet, ref height, ref idx, ChangeHeight, DefaultRowH);

                        rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, idx);
                        rowHeights[currentChar] = height;
                    }

                    rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowIndexes[currentChar].Item2 + 1);
                    rowHeights[currentChar] += templateExtrahight;

                    string currentSheet = worksheet.Name.Split(" ")[1];
                    sheet.Cells[$"H{rowIndexes[currentSheet].Item2}"].Style.Numberformat.Format = "0.0#";
                    sheet.Cells[$"I{rowIndexes[currentSheet].Item2}"].Style.Numberformat.Format = "0.0##";

                    sheet.Cells[$"H{rowIndexes[currentSheet].Item2}"].Formula = $"SUM(H{rowIndexes[currentSheet].Item1}:H{rowIndexes[currentSheet].Item2 - 1})";
                    sheet.Cells[$"I{rowIndexes[currentSheet].Item2}"].Formula = $"SUM(I{rowIndexes[currentSheet].Item1}:I{rowIndexes[currentSheet].Item2 - 1})";
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

                Dictionary<string, int> abnormalStarts = rowIndexes.ToDictionary(kpv => kpv.Key,kpv => kpv.Value.Item2);

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

                                abnormalStarts.Add(nextPage, idx);
                            }
                            rowIndexes[currentChar] = (rowIndexes[currentChar].Item1, rowindex - 1);
                            rowHeights[currentChar] += specialTotalHeight;

                            specialTotalHeight = 0;

                            rowindex = startingAbnormalRow = abnormalStarts[nextPage];
                        }
                    }
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

                if (SosAnalysis.Illustrations != null && SosAnalysis.Illustrations.Any()) {

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
                                        else if (h + changeableOff > changeableH )
                                        {
                                            double overflow = (h + changeableOff) - changeableH; 
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
                                                    abnormalStarts.Add(nextPage,idx);

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
                                    
                                    horizontalOffset = (int)((changeableW - w) / 2);
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

                            picture.SetPosition((int)globalYoffset + offsetY,(int)globalXoffset + horizontalOffset);

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

                //if (changedAnalysis || changedSheet || changedImgSheet)
                //{
                //    sheet = package.Workbook.Worksheets["Analysis B"];
                //    double specialHeight = stylesService.CalculateRowHeight(SosAnalysis.SOSHub.OtherInformation, (sheet.Columns[2].Width + sheet.Columns[3].Width + sheet.Columns[4].Width), sheet.Cells[$"B{startingAbnormalRowB}"].Style.Font.Size);
                //    int rows = leftoffrow - startingAbnormalRowB + 1;
                //    int size = 20 * rows;
                //    if (specialHeight > size)
                //    {
                //        double nRowH = (specialHeight - size) / rows;
                //        for (int i = startingAbnormalRowB; i <= leftoffrow; i++)
                //        {
                //            sheet.Rows[i].Height += nRowH;
                //        }
                //    }
                //    sheet.Cells[$"B{startingAbnormalRowB}"].Value = SosAnalysis.SOSHub.OtherInformation;
                //}
                //if (sheet.Name == "Analysis B")
                //    sheet = package.Workbook.Worksheets["Analysis A"];

                //double spHeight = stylesService.CalculateRowHeight(SosAnalysis.SOSHub.OtherInformation, (sheet.Columns[2].Width + sheet.Columns[3].Width + sheet.Columns[4].Width), sheet.Cells[$"B{startingAbnormalRow}"].Style.Font.Size);
                //int spRows = leftoffrow - startingAbnormalRowB + 1;
                //int tSize = 20 * spRows;
                //if (spHeight > tSize)
                //{
                //    double nRowH = (spHeight - tSize) / spRows;
                //    for (int i = startingAbnormalRow; i <= rowindex; i++)
                //    {
                //        sheet.Rows[i].Height += nRowH;
                //    }
                //}
                //sheet.Cells[$"B{startingAbnormalRow}"].Value = SosAnalysis.SOSHub.OtherInformation;

                package.Workbook.Worksheets.First().Select();

                #endregion

                #endregion

                // Save to file
                //package.Workbook.Calculate();

                sheet.Protection.IsProtected = true;
                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", !string.IsNullOrEmpty(SosAnalysis.InternalControlNumber)? $"{SosAnalysis.InternalControlNumber} Analysis Report.xlsx" : "Analysis Report.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }

        [HttpGet("Excel/Secuence/{SecuenceId}")]
        public async Task<IActionResult> SecuenceExcelExport(int SecuenceId)
        {
            var SosAnalysis = await _AnalysisProcessRepository.GetSOSAnalysis(SecuenceId, true, true, true, true, true, true);

            string templateName = "DataAccess/Templates/Secuence Template.xlsx";
            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(templateName);

            using (var package = new ExcelPackage(templateStream))
            {
                var sheet  = package.Workbook.Worksheets.First();

                #region information table

                sheet.Cells["D6"].Value = SosAnalysis.OperationName;
                sheet.Cells["D8"].Value = SosAnalysis.InternalControlNumber;
                sheet.Cells["G8"].Value = SosAnalysis.ProcessName;
                string SecurityEq = "";
                if (SosAnalysis.SOSHub?.SafetyEquipment != null && SosAnalysis.SOSHub.SafetyEquipment.Any())
                {
                    SecurityEq = string.Join(", ", SosAnalysis.SOSHub.SafetyEquipment.Select(se => se.EquipmentName));
                }
                sheet.Cells["D9"].Value = SecurityEq;
                string Tools = "";
                if (SosAnalysis.SOSHub?.ToolsUsed != null && SosAnalysis.SOSHub.ToolsUsed.Any())
                {
                    Tools = string.Join(", ", SosAnalysis.SOSHub.ToolsUsed.Select(tu => $"{tu.Tool.ToolName} ({tu.Quantity})"));
                }
                sheet.Cells["D10"].Value = Tools;
                sheet.Cells["D11"].Value = SosAnalysis.SOSHub.AppliedModel?.Description;
                sheet.Cells["D12"].Value = SosAnalysis.SOSHub.TrainingTime;

                #region revitions

                if (SosAnalysis.AnalysisLogbooks != null && SosAnalysis.AnalysisLogbooks.Any())
                {
                    //SosAnalysis.AnalysisLogbooks = SosAnalysis.AnalysisLogbooks?.OrderByDescending(p => p.NoRevision).ToList();

                    List<string> Cols = new List<string> { "K", "N", "O", "P" };

                    //foreach (var (item, index) in SosAnalysis.AnalysisLogbooks.Take(4).Select((item, index) => (item, index)))
                    //{
                    //    sheet.Cells[$"{Cols[index]}6"].Value = item == SosAnalysis.AnalysisLogbooks.First() ? "N" : item.NoRevision;
                    //    sheet.Cells[$"{Cols[index]}7"].Value = item.Date?.ToString("dd-MMM-yyyy").Replace(".", "");
                    //    sheet.Cells[$"{Cols[index]}8"].Value = item.RevisedItem;
                    //    sheet.Cells[$"{Cols[index]}11"].Value = item.SeniorSupervisor.Name;
                    //    sheet.Cells[$"{Cols[index]}12"].Value = item.Supervisor.Name;
                    //}

                    //if (SosAnalysis.AnalysisLogbooks.Skip(4).Any())
                    //{
                    //    sheet = package.Workbook.Worksheets[2];
                    //    int backuprow = 3;
                    //    foreach (var item in SosAnalysis.AnalysisLogbooks.Skip(4))
                    //    {
                    //        stylesService.BackupRowStyle(sheet, backuprow);
                    //        sheet.Cells[$"A{backuprow}"].Value = item.NoRevision;
                    //        sheet.Cells[$"B{backuprow}"].Value = item.Date;
                    //        sheet.Cells[$"C{backuprow}"].Value = item.RevisedItem;
                    //        sheet.Cells[$"D{backuprow}"].Value = item.SeniorSupervisor.Name;
                    //        sheet.Cells[$"E{backuprow}"].Value = item.Supervisor.Name;
                    //        backuprow++;
                    //    }
                    //    sheet = package.Workbook.Worksheets.First();
                    //}
                }

                #endregion

                #endregion

                #region analyses

                const int row13 = 15;//to propperly merge rows in the illustration column
                const int sheetBrow6 = 6; //to propperly merge in the illustation column in Sheet B
                double TotalRowHight = 0;//to be able to know when to jump to next sheet
                double TotalRowHightA = 15;
                double TotalRowHightB = 15;
                bool changedAnalysis = false;
                const int ChangeHeight = 580;//Total row height from an empty template to change sheet

                int startingRow = 16;//the row where the analyses start in an empty template
                int startingRowB = 7;

                int sheetStartRow = startingRow;

                int rowindex = startingRow;//to get where the final row ended
                int leftoffrow = 0;

                int indexAnalysis = 0;
                int tableIndexAnalysis = 0;
                int indexSection = 0;

                var ValuesFont = sheet.Cells["D6"].Style.Font;

                if (SosAnalysis.SOSHub.Sections != null && SosAnalysis.SOSHub.Sections.Any())
                {
                    foreach (var section in SosAnalysis.SOSHub.Sections)
                    {
                        indexSection++;

                        int comparator = 0;
                        if (section.Analyses.Count % 2 == 0)
                            comparator = (section.Analyses.Count / 2) - 1;
                        else
                            comparator = section.Analyses.Count / 2;

                        foreach (var (analysis, index) in section.Analyses.Select((analysis, index) => (analysis, index)))
                        {
                            double analysisHeight, StepHeight, CriticalHeight;
                            indexAnalysis++;
                            rowindex = sheetStartRow + tableIndexAnalysis++;
                            if (analysis != SosAnalysis.SOSHub.Sections.First().Analyses.First() && analysis != SosAnalysis.SOSHub.Sections.Last().Analyses.Last())
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
                                if (!string.IsNullOrEmpty(section.Time))
                                {
                                    string[] times = section.Time.Split('.');
                                    double minutes = double.Parse(times[0]) / 60;
                                    sheet.Cells[$"H{rowindex}"].Style.Numberformat.Format = "0.##";
                                    sheet.Cells[$"I{rowindex}"].Style.Numberformat.Format = "0.###";

                                    sheet.Cells[$"H{rowindex}"].Value = minutes;
                                    if (times.Length > 1)
                                        sheet.Cells[$"I{rowindex}"].Value = double.Parse(times[1]) / 100;
                                }
                            }

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

                                    sheet.Cells[$"J{rowindex}"].RichText.Add(indexString);
                                    sheet.Cells[$"J{rowindex}"].RichText.Add(critString);
                                    sheet.Cells[$"J{rowindex}"].RichText.Add(reasonString);

                                    fullText += $"{indexString}{critString}{reasonString}";
                                }
                            }

                            analysisHeight = stylesService.CalculateRowHeight(analysis.Text, sheet.Columns[3].Width + sheet.Columns[4].Width, sheet.Cells["B14"].Style.Font.Size);
                            StepHeight = stylesService.CalculateRowHeight(section.Step, (sheet.Columns[6].Width + sheet.Columns[7].Width), sheet.Cells["F14"].Style.Font.Size);
                            CriticalHeight = stylesService.CalculateRowHeight(fullText, (sheet.Columns[10].Width + sheet.Columns[11].Width + sheet.Columns[12].Width), sheet.Cells["J14"].Style.Font.Size);

                            var rowheight = Math.Max(20, Math.Max(analysisHeight, Math.Max(StepHeight, CriticalHeight)));

                            TotalRowHight += rowheight;

                            sheet.Rows[rowindex].Height = rowheight;

                            if (TotalRowHight >= ChangeHeight && !changedAnalysis)
                            {
                                sheet = package.Workbook.Worksheets["Analysis B"];
                                TotalRowHightA += TotalRowHight;
                                TotalRowHight = 0;
                                leftoffrow = rowindex;
                                sheetStartRow = rowindex = startingRowB;
                                tableIndexAnalysis = 0;
                                changedAnalysis = true;
                            }
                        }
                    }
                    double templateExtrahight = 25.1 + 8 + 16.2;

                    int rowSpaceA = 17, rowSpaceB = 10;

                    if (changedAnalysis)
                    {
                        rowindex++;
                        sheet.Cells[$"H{rowindex}"].Style.Numberformat.Format = "0.0#";
                        sheet.Cells[$"I{rowindex}"].Style.Numberformat.Format = "0.0##";

                        sheet.Cells[$"H{rowindex}"].Formula = $"SUM(H{startingRowB}:H{rowindex - 1})";
                        sheet.Cells[$"I{rowindex}"].Formula = $"SUM(I{startingRowB}:I{rowindex - 1})";
                        rowSpaceB = rowindex + 1;
                        //to return to package A
                        sheet = package.Workbook.Worksheets["Analysis A"];
                        //save the rowindex in sheet B
                        var temp = rowindex;
                        //return to row in Sheet A
                        rowindex = leftoffrow;
                        //save the rowindex of sheet B in case whe need to write more special cases
                        leftoffrow = temp;
                        //save rowheight
                        TotalRowHightB += TotalRowHight + templateExtrahight;
                        TotalRowHightA += templateExtrahight;
                    }
                    else
                    {
                        TotalRowHightA += TotalRowHight + templateExtrahight;
                        TotalRowHightB += templateExtrahight;
                    }
                    rowindex++;

                    sheet.Cells[$"H{rowindex}"].Style.Numberformat.Format = "0.0#";
                    sheet.Cells[$"I{rowindex}"].Style.Numberformat.Format = "0.0##";

                    sheet.Cells[$"H{rowindex}"].Formula = $"SUM(H{startingRow}:H{rowindex - 1})";
                    sheet.Cells[$"I{rowindex}"].Formula = $"SUM(I{startingRow}:I{rowindex - 1})";
                    rowSpaceA = rowindex + 1;
                }
                else
                {
                    do
                    {
                        sheet.InsertRow(rowindex, 1);
                        stylesService.ApplyAnalysisStyles(sheet, rowindex, false);
                        rowindex++;
                    } while (rowindex < 29);
                    TotalRowHight = ChangeHeight;
                }

                #endregion

                package.Workbook.Worksheets.First().Select();
                //sheet.Protection.IsProtected = true;
                package.SaveAs(ms);
            }
            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", !string.IsNullOrEmpty(SosAnalysis.InternalControlNumber) ? $"{SosAnalysis.InternalControlNumber} Analysis Report.xlsx" : "Analysis Report.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }
    }
}

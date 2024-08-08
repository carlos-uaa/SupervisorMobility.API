using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/Exportation")]
    [ApiController]
    public class ExportationController : ControllerBase
    {
        private readonly ISOSAnalysis_ProcessRepository _AnalysisProcessRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ExportationStylesService stylesService;
        private readonly ExportationImgService imgService;

        public ExportationController(ISOSAnalysis_ProcessRepository repository, IWebHostEnvironment env)
        {
            _AnalysisProcessRepository = repository;
            _env = env ?? throw new ArgumentNullException(nameof(env));
            stylesService = new ExportationStylesService();
            imgService = new ExportationImgService();
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
                    Tools = string.Join(", ", SosAnalysis.SOSHub.ToolsUsed.Select(tu => tu.Tool.ToolName));
                }
                sheet.Cells["D8"].Value = Tools;
                sheet.Cells["D9"].Value = SosAnalysis.SOSHub.AppliedModel?.Description;
                sheet.Cells["D10"].Value = SosAnalysis.SOSHub.TrainingTime;

                #region revitions

                if (SosAnalysis.AnalysisLogbooks != null && SosAnalysis.AnalysisLogbooks.Any())
                {
                    SosAnalysis.AnalysisLogbooks = SosAnalysis.AnalysisLogbooks?.OrderByDescending(p => p.NoRevision).ToList();

                    List<string> Cols = new List<string>{ "K", "N", "O", "P" };

                    foreach (var (item, index) in SosAnalysis.AnalysisLogbooks.Take(4).Select((item, index)=>(item, index)))
                    {
                        sheet.Cells[$"{Cols[index]}4"].Value = item.NoRevision;
                        sheet.Cells[$"{Cols[index]}5"].Value = item.Date;
                        sheet.Cells[$"{Cols[index]}6"].Value = item.RevisedItem;
                        sheet.Cells[$"{Cols[index]}9"].Value = item.SeniorSupervisor.Name;
                        sheet.Cells[$"{Cols[index]}10"].Value = item.Supervisor.Name;
                    }

                    if (SosAnalysis.AnalysisLogbooks.Skip(4).Any())
                    {
                        sheet = package.Workbook.Worksheets["Backup"];
                        int backuprow = 3;
                        foreach (var item in SosAnalysis.AnalysisLogbooks.Skip(4))
                        {
                            stylesService.BackupRowStyle(sheet, backuprow);
                            sheet.Cells[$"A{backuprow}"].Value = item.NoRevision;
                            sheet.Cells[$"B{backuprow}"].Value = item.Date;
                            sheet.Cells[$"C{backuprow}"].Value = item.RevisedItem;
                            sheet.Cells[$"D{backuprow}"].Value = item.SeniorSupervisor.Name;
                            sheet.Cells[$"E{backuprow}"].Value = item.Supervisor.Name;
                            backuprow++;
                        }
                        sheet = package.Workbook.Worksheets["Analysis A"];
                    }
                }

                #endregion

                #endregion

                #region analyses

                const int row13 = 13;//to propperly merge rows in the illustration column
                const int sheetBrow6 = 6; //to propperly merge in the illustation column in Sheet B
                double TotalRowHight = 0;//to be able to know when to jump to next sheet
                double TotalRowHightA = 0;
                double TotalRowHightB = 0;
                bool changedAnalysis = false;
                const int ChangeHeight = 580;//Total row height from an empty template to change sheet

                int startingRow = 14;//the row where the analyses start in an empty template
                int startingRowB = 7;

                int sheetStartRow = startingRow;

                int rowindex = startingRow;//to get where the final row ended
                int leftoffrow = 0;

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
                                sheet.Cells[$"H{rowindex}"].Value = section.Time.Split(".")[0];
                                sheet.Cells[$"I{rowindex}"].Value = section.Time.Split(".")[1];
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

                        TotalRowHight += rowheight;

                        sheet.Rows[rowindex].Height = rowheight;

                        if(TotalRowHight >= ChangeHeight && !changedAnalysis)
                        {
                            sheet = package.Workbook.Worksheets["Analysis B"];
                            TotalRowHightA = TotalRowHight;
                            TotalRowHight = 0;
                            leftoffrow = rowindex;
                            sheetStartRow = rowindex = startingRowB;
                            tableIndexAnalysis = 0;
                            changedAnalysis = true;
                        }
                    }
                }
                if (changedAnalysis)
                {
                    //to return to package A
                    sheet = package.Workbook.Worksheets["Analysis A"];
                    //save the rowindex in sheet B
                    var temp = rowindex;
                    //return to row in Sheet A
                    rowindex = leftoffrow;
                    //save the rowindex of sheet B in case whe need to write more special cases
                    leftoffrow = temp;
                    //save rowheight
                    TotalRowHightB = TotalRowHight;
                }
                else
                {
                    TotalRowHightA = TotalRowHight;
                }
                rowindex++;
                double totalTime = SosAnalysis.SOSHub.Sections
                            .Select(sect =>
                            {
                                double timeValue;
                                return double.TryParse(sect.Time, out timeValue) ? timeValue : (double?)null;
                            })
                            .Where(timeValue => timeValue.HasValue)
                            .Select(timeValue => timeValue.Value)
                            .DefaultIfEmpty(0.0)
                            .Sum();
                string[] splittedTime = totalTime.ToString().Split(".");

                sheet.Cells[$"H{rowindex}"].Value = splittedTime[0];
                sheet.Cells[$"I{rowindex}"].Value = splittedTime.Length == 2? splittedTime[1] : splittedTime[0];



                #endregion

                #region Abnormal cases

                rowindex+=3;

                int startingAbnormalRow = rowindex;
                int startingAbnormalRowB = leftoffrow == 0? 12 : leftoffrow==7? leftoffrow + 5 : leftoffrow + 4;
                bool changedSheet = false;

                if (SosAnalysis.SOSHub.MaterialsUsed != null && SosAnalysis.SOSHub.MaterialsUsed.Any())
                {
                    foreach (var item in SosAnalysis.SOSHub.MaterialsUsed)
                    {
                        sheet.InsertRow(rowindex, 1);
                        bool last = item == SosAnalysis.SOSHub.MaterialsUsed.Last();
                        stylesService.ApplySpecialCasesRowStyle(sheet, rowindex, last);

                        sheet.Cells[$"E{rowindex}"].Value = item.Material.key;
                        sheet.Cells[$"F{rowindex}"].Value = item.Material.PartName;
                        sheet.Cells[$"H{rowindex}"].Value = item.Material.PartNumber;
                        sheet.Cells[$"K{rowindex}"].Value = item.Quantity;

                        rowindex++;

                        if (rowindex - startingAbnormalRow >= 8)
                        {
                            changedSheet = true;
                            sheet = package.Workbook.Worksheets["Analysis B"];
                            var temp = leftoffrow;
                            leftoffrow = rowindex;
                            rowindex = temp == 7 ? temp + 5 : temp + 4;
                            startingAbnormalRowB = rowindex;
                        }
                    }
                    if (!changedSheet)
                    {
                        int i, max;
                        if (leftoffrow == 0 || leftoffrow == 7) { i = 12; max = 15; leftoffrow = 14;}
                        else { i = leftoffrow + 4; max = i + 3; }
                        sheet = package.Workbook.Worksheets["Analysis B"];
                        do
                        {
                            sheet.InsertRow(i, 1);
                            bool last = i + 1 >= max;
                            stylesService.ApplySpecialCasesRowStyle(sheet, i, last);
                            i++;
                        } while (i < max);

                        stylesService.SetSpecialCasesFirstColumnStyle(sheet, startingAbnormalRowB, leftoffrow);
                        sheet = package.Workbook.Worksheets["Analysis A"];
                    }
                }
                else
                {
                    int max = rowindex + 3;
                    do
                    {
                        sheet.InsertRow(rowindex, 1);
                        bool last = rowindex + 1 >= max;
                        stylesService.ApplySpecialCasesRowStyle(sheet, rowindex, last);
                        rowindex++;
                    } while (rowindex < max);
                    int i = 0;
                    if (leftoffrow == 0 || leftoffrow == 7) { i = 12; max = 15; leftoffrow = 14; }
                    else { i = leftoffrow + 4; max = i + 3; }
                    sheet = package.Workbook.Worksheets["Analysis B"];
                    do
                    {
                        sheet.InsertRow(i, 1);
                        bool last = i + 1 >= max;
                        stylesService.ApplySpecialCasesRowStyle(sheet, i, last);
                        i++;
                    } while (i < max);

                    stylesService.SetSpecialCasesFirstColumnStyle(sheet, startingAbnormalRowB, leftoffrow);
                    sheet = package.Workbook.Worksheets["Analysis A"];
                }

                if (changedSheet)
                {
                    rowindex--;
                    stylesService.SetSpecialCasesFirstColumnStyle(sheet, startingAbnormalRowB, rowindex);
                    sheet = package.Workbook.Worksheets["Analysis A"];
                    var temp = leftoffrow;
                    leftoffrow = rowindex;
                    rowindex = temp;
                    tableIndexAnalysis = 0;
                }

                rowindex--;

                stylesService.SetSpecialCasesFirstColumnStyle(sheet, startingAbnormalRow, rowindex);

                sheet.Cells[$"M{row13}:P{rowindex}"].Merge = true;

                sheet.Cells[$"M{row13}:P{rowindex}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                sheet.Cells[$"M{row13}:P{rowindex}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                double imgCellWidthA = sheet.Columns[13].Width + sheet.Columns[14].Width + sheet.Columns[15].Width + sheet.Columns[16].Width;
                
                sheet = package.Workbook.Worksheets["Analysis B"];

                sheet.Cells[$"M{sheetBrow6}:M{leftoffrow}"].Merge = true;

                sheet.Cells[$"M{sheetBrow6}:M{leftoffrow}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                sheet.Cells[$"M{sheetBrow6}:M{leftoffrow}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                double imgCellWidthB = sheet.Columns[13].Width;

                #region images

                sheet = package.Workbook.Worksheets["Analysis A"];

                if (SosAnalysis.Illustrations != null && SosAnalysis.Illustrations.Any()) {

                    string[] imgPath = { $"uploads/SOSAnalysis/Ilustrations/", "" };


                    int _case = SosAnalysis.Illustrations.Count > 2 ? 1 : 0;

                    double changeHeightP = imgService.HeightToPixels(TotalRowHightA);

                    int currentSheetColumnWidth = imgService.WidthToPixels(imgCellWidthA);

                    bool add2ndImg = false;
                    bool changedImgSheet = false;

                    int offsetY = 2;

                    double globalXoffset = imgService.WidthToPixels(175.39)+20/*169.39*/, globalYoffset = imgService.HeightToPixels(295.2)+20/*280.2*/;

                    int tempindex = 0;
                    foreach (var image in SosAnalysis.Illustrations)
                    {
                        if (offsetY >= changeHeightP - 150 && !changedImgSheet)
                        {
                            sheet = package.Workbook.Worksheets["Analysis B"];
                            //changeHeightP = TotalRowHightB >= 100 ?  : ; to change height maybe
                            currentSheetColumnWidth = imgService.WidthToPixels(imgCellWidthB);
                            offsetY = 2;
                            changedImgSheet = true;
                        }

                        imgPath[1] = image.StorageFileName;
                        int horizontalOffset = 0;
                        using (FileStream stream = System.IO.File.OpenRead($"{imgPath[0]}{imgPath[1]}"))
                        {
                            Image imgObj = Image.FromStream(stream);
                            int w = 0, h = 0;

                            switch (_case)
                            {
                                case 0:
                                    if(imgObj.Height > changeHeightP || imgObj.Width > currentSheetColumnWidth)
                                    {
                                        //if(imgObj.Height == imgObj.Width)
                                        //{
                                        //    //imgObj = imgService.ResizeImage(imgObj, currentSheetColumnWidth, (int)changeHeightP);
                                        //    (w, h) = (currentSheetColumnWidth, (int)changeHeightP);
                                        //}
                                        if(imgObj.Width > imgObj.Height)
                                        {
                                            //imgObj = imgService.ResizeImageMaintainingAspectRatio(imgObj, currentSheetColumnWidth, true);
                                            (w, h) = imgService.GetResizeMagnitudesMaintainingAspectRatio(imgObj, currentSheetColumnWidth, true);
                                        }
                                        else
                                        {
                                            //imgObj = imgService.ResizeImageMaintainingAspectRatio(imgObj, (int)changeHeightP, false);
                                            (w, h) = imgService.GetResizeMagnitudesMaintainingAspectRatio(imgObj, (int)changeHeightP, false);
                                        }
                                    }
                                    //else if(imgObj.Width > currentSheetColumnWidth)
                                    //{
                                    //    //imgObj = imgService.ResizeImageMaintainingAspectRatio(imgObj, currentSheetColumnWidth, true);
                                    //    (w, h) = imgService.GetResizeMagnitudesMaintainingAspectRatio(imgObj, currentSheetColumnWidth, true);
                                    //}
                                    //else if (imgObj.Height > changeHeightP)
                                    //{
                                    //    (w, h) = imgService.GetResizeMagnitudesMaintainingAspectRatio(imgObj, (int)changeHeightP, false);
                                    //}
                                    //Horizontal offset
                                    horizontalOffset = (int)((currentSheetColumnWidth - w) / 2);
                                    break;
                                case 1:
                                    //imgObj = imgService.ResizeImage(imgObj, (currentSheetColumnWidth / 2) - 20, 160);
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

                            //imgObj.Save($"Temp/{tempindex}.png");

                            //FileInfo fileInfo = new FileInfo($"Temp/{tempindex}.png");

                            string pictureName = image.FileName.Split(".")[0];
                            var picture = sheet.Drawings.AddPicture($"{pictureName}{tempindex}.png", stream);
                            picture.SetSize(w, h);

                            if (!changedImgSheet)
                            {
                                picture.SetPosition((int)globalYoffset + offsetY,(int)globalXoffset + horizontalOffset);
                            }
                            else
                            {
                                picture.SetPosition(sheetBrow6-1, offsetY, 12, horizontalOffset);
                            }

                            if (_case == 0)
                            {
                                offsetY += h;
                            }
                            else if (!add2ndImg)
                            {
                                offsetY += h + 5;
                            }

                            tempindex++;
                        }
                    }

                }

                #endregion
                
                #endregion

                // Save to file
                sheet.Protection.IsProtected = true;
                package.SaveAs(ms);
            }

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", !string.IsNullOrEmpty(SosAnalysis.InternalControlNumber)? $"{SosAnalysis.InternalControlNumber} Analysis Report.xlsx" : "Analysis Report.xlsx");
            res.EnableRangeProcessing = true;
            return res;
        }

    }
}

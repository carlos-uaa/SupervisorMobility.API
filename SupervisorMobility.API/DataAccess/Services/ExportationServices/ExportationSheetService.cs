using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Text.RegularExpressions;

namespace SupervisorMobility.API.DataAccess.Services.ExportationServices
{
    public class ExportationSheetService
    {
        private readonly ExportationStylesService _stylesService;
        public ExportationSheetService() 
        {
            _stylesService = new ExportationStylesService();
        }

        public void AddSheet(ExcelPackage package, int type, string currentIdx = "A")
        {
            string sheetName = "";
            string sheetWN = "";
            switch (type)
            {
                case 0:
                    sheetName = "DataAccess/Templates/Backup Template.xlsx";
                    sheetWN = "Backup";
                    break;
                case 1:
                    sheetName = "DataAccess/Templates/Analysis Extra Template.xlsx";
                    sheetWN = "Analysis " + GetNextCombination(currentIdx);
                    break;
                case 2:
                    sheetName = "DataAccess/Templates/Sequence Extra Template.xlsx";
                    sheetWN = "Sequence " + GetNextCombination(currentIdx);
                    break;
                case 3:
                    sheetName = "DataAccess/Templates/Distribution Extra Template.xlsx";
                    sheetWN = "HOE DISTRIBUTION (" + GetNextIndex(currentIdx) + ")";
                    break;
            }

            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(sheetName);

            using (var ExtraTemplate = new ExcelPackage(templateStream))
            {
                ExcelWorksheet sourceSheet = ExtraTemplate.Workbook.Worksheets[0];

                package.Workbook.Worksheets.Add(sheetWN, sourceSheet);

                switch (type)
                {
                    case 1:
                    case 2:
                        if (package.Workbook.Worksheets.Any(ws => ws.Name == "Backup"))
                            package.Workbook.Worksheets.MoveBefore(sheetWN, "Backup");
                        break;
                    case 3:
                        //var drawing = package.Workbook.Worksheets[sheetWN].Drawings.First(p=>p.Name == "Picture 2");
                        //drawing.SetSize(-50);
                        break;
                }
            }

            templateStream.Close();
            ms.Close();
            templateStream.Dispose();
            ms.Dispose();
        }

        public void AddOtherSheet(ExcelPackage package, int type, string currentIdx = "A")
        {
            string sheetName = "";
            string sheetWN = "";
            switch (type)
            {
                case 0:
                    sheetName = "DataAccess/Templates/PAT Yearly template.xlsx";
                    sheetWN = "SOS Anual " + GetNextIndex(currentIdx);
                    break;
                default:
                    break;
            }

            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(sheetName);

            using (var ExtraTemplate = new ExcelPackage(templateStream))
            {
                ExcelWorksheet sourceSheet = ExtraTemplate.Workbook.Worksheets[0];

                package.Workbook.Worksheets.Add(sheetWN, sourceSheet);

            }

            templateStream.Close();
            ms.Close();
            templateStream.Dispose();
            ms.Dispose();
        }

        public void GenerateAnalysisRows(ExcelWorksheet worksheet, ref double rowHeight, ref int idx, double ChangeHeight, double DefaultRowH)
        {
            _stylesService.ChangeLastRowStyleAnalysis(worksheet, idx, false);

            if (rowHeight == 0)
                rowHeight = DefaultRowH;

            while (rowHeight < ChangeHeight)
            {
                double doitFit = (rowHeight) * 100 / ChangeHeight;
                if (doitFit < 95)
                {
                    worksheet.InsertRow(++idx, 1);
                    _stylesService.ApplyAnalysisStyles(worksheet, idx, false);
                    rowHeight += 20;
                }
                else
                {
                    _stylesService.ChangeLastRowStyleAnalysis(worksheet, idx, true);
                    break;
                }
            }
        }

        public void GenerateSequenceRows(ExcelWorksheet worksheet, ref double rowHeight, ref int idx, double ChangeHeight, double DefaultRowH)
        {
            _stylesService.ChangeLastRowStyleSequence(worksheet, idx, false);

            if (rowHeight == 0)
                rowHeight = DefaultRowH;

            while (rowHeight < ChangeHeight)
            {
                double doitFit = (rowHeight) * 100 / ChangeHeight;
                if (doitFit < 95)
                {
                    worksheet.InsertRow(++idx, 1);
                    _stylesService.ApplySequenceStyles(worksheet, idx, false);
                    rowHeight += 20;
                }
                else
                {
                    _stylesService.ChangeLastRowStyleSequence(worksheet, idx, true);
                    break;
                }
            }
        }

        public void GenerateDistributionsRows(ExcelWorksheet worksheet, ref double rowHeight, ref int idx, double ChangeHeight, double DefaultRowH, bool isFirst)
        {
            if (rowHeight == 0)
                rowHeight = DefaultRowH;

            while (rowHeight < ChangeHeight)
            {
                double doitFit = (rowHeight) * 100 / ChangeHeight;
                if (doitFit < 95)
                {
                    worksheet.InsertRow(++idx, 1);
                    _stylesService.ApplyDistributionStyles(worksheet, idx, isFirst);
                    rowHeight += 21.8;
                }
                else
                {
                    break;
                }
            }
        }

        public void GenerateAbnormalRows(ExcelWorksheet worksheet, ref double rowHeight, ref int idx, int startIdx, bool empty = false)
        {
            if(empty)
                _stylesService.ApplySpecialCasesRowStyle(worksheet, idx, false);
            idx++;
            for (; idx < startIdx + 8; idx++)
            {
                worksheet.InsertRow(idx, 1);
                rowHeight += 20;
                bool last = idx - startIdx >= 7;
                _stylesService.ApplySpecialCasesRowStyle(worksheet, idx, last);
            }
            idx--;
        }

        public string GetNextCombination(string input)
        {
            char[] chars = input.ToCharArray();
            int length = chars.Length;

            for (int i = length - 1; i >= 0; i--)
            {
                if (chars[i] < 'Z')
                {
                    chars[i]++;
                    return new string(chars);
                }
                chars[i] = 'A';
            }

            return new string('A', length + 1);
        }

        public int ColumnLetterToNumber(string columnLetters)
        {
            int sum = 0; 
            foreach (char c in columnLetters) { sum *= 26; sum += c - 'A' + 1; }
            return sum;
        }

        public int GetNextIndex(string input)
        {
            string pattern = @"\d+";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                int number = int.Parse(match.Value) + 1;
                return number;
            }
            else
            {
                return 2;
            }
        }

        public void MoveRowValuesDistribution(ExcelWorksheet sheet, int rowindex, bool isFirst)
        {
            sheet.Rows[rowindex].Height = sheet.Rows[rowindex - 1].Height;
            sheet.Cells[$"B{rowindex}"].Value = sheet.Cells[$"B{rowindex - 1}"].Value;
            sheet.Cells[$"C{rowindex}"].Value = sheet.Cells[$"C{rowindex - 1}"].Value;
            sheet.Cells[$"H{rowindex}"].Value = sheet.Cells[$"H{rowindex - 1}"].Value;
            sheet.Cells[$"L{rowindex}"].Value = sheet.Cells[$"L{rowindex - 1}"].Value;
            sheet.Cells[$"M{rowindex}"].Value = sheet.Cells[$"M{rowindex - 1}"].Value;
            sheet.Cells[$"N{rowindex}"].Value = sheet.Cells[$"N{rowindex - 1}"].Value;
            sheet.Cells[$"O{rowindex}"].Value = sheet.Cells[$"O{rowindex - 1}"].Value;
            if (isFirst)
            {
                sheet.Cells[$"Q{rowindex}"].Value = sheet.Cells[$"Q{rowindex - 1}"].Value;
            }
            else
            {
                sheet.Cells[$"P{rowindex}"].Value = sheet.Cells[$"P{rowindex - 1}"].Value;
            }
        }

        public void SetPrintingOptions(ExcelWorkbook workbook)
        {
            foreach (ExcelWorksheet worksheet in workbook.Worksheets)
            {
                worksheet.PrinterSettings.PaperSize = ePaperSize.Tabloid;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.TopMargin = 0.5m;
                worksheet.PrinterSettings.BottomMargin = 0.5m;
                worksheet.PrinterSettings.LeftMargin = 0.5m;
                worksheet.PrinterSettings.RightMargin = 0.5m;
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.FitToHeight = 1;
                worksheet.PrinterSettings.HorizontalCentered = true;
                worksheet.Protection.IsProtected = true;
            }
        }
    }
}

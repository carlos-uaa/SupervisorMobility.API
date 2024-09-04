using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;

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
                    sheetName = "Sequence " + GetNextCombination(currentIdx);
                    break;
            }

            MemoryStream ms = new MemoryStream();

            using var templateStream = System.IO.File.OpenRead(sheetName);

            using (var ExtraTemplate = new ExcelPackage(templateStream))
            {
                ExcelWorksheet sourceSheet = ExtraTemplate.Workbook.Worksheets[0];
                ExcelWorksheet newSheet = package.Workbook.Worksheets.Add(sheetWN, sourceSheet);
                if (type >= 1 && type <= 2)
                {
                    if (package.Workbook.Worksheets.Any(ws => ws.Name == "Backup"))
                        package.Workbook.Worksheets.MoveBefore(sheetWN, "Backup");
                }
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
            }
        }
    }
}

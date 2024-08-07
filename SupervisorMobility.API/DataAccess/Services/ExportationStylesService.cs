using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace SupervisorMobility.API.DataAccess.Services
{
    public class ExportationStylesService
    {
        public ExportationStylesService() { }

        public void ApplyAnalysisStyles(ExcelWorksheet sheet, int rownumber, bool last)
        {
            sheet.Rows[rownumber].Height = 20;

            sheet.Cells[$"B{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"B{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"B{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"B{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"B{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"C{rownumber}:D{rownumber}"].Merge = true;
            sheet.Cells[$"C{rownumber}:D{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{rownumber}:D{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{rownumber}:D{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"C{rownumber}:D{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"C{rownumber}:D{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            sheet.Cells[$"E{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"E{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"E{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"E{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"E{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"F{rownumber}:G{rownumber}"].Merge = true;
            sheet.Cells[$"F{rownumber}:G{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"F{rownumber}:G{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"F{rownumber}:G{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"F{rownumber}:G{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"F{rownumber}:G{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"H{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"H{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Dashed;
            sheet.Cells[$"H{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"H{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"H{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"I{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Dashed;
            sheet.Cells[$"I{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"I{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"I{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"I{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"J{rownumber}:L{rownumber}"].Merge = true;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        public void SetSpecialCasesFirstColumnStyle(ExcelWorksheet sheet, int initialrow, int lastrow)
        {
            sheet.Cells[$"B{initialrow}:D{lastrow}"].Merge = true;
            sheet.Cells[$"B{initialrow}:D{lastrow}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"B{initialrow}:D{lastrow}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        }

        public void ApplySpecialCasesRowStyle(ExcelWorksheet sheet, int row, bool last)
        {
            sheet.Cells[$"E{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"E{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"E{row}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Thin : ExcelBorderStyle.Medium;
            sheet.Cells[$"E{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"F{row}:G{row}"].Merge = true;
            sheet.Cells[$"F{row}:G{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"F{row}:G{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"F{row}:G{row}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Thin : ExcelBorderStyle.Medium;
            sheet.Cells[$"F{row}:G{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"F{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            sheet.Cells[$"H{row}:J{row}"].Merge = true;
            sheet.Cells[$"H{row}:J{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"H{row}:J{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"H{row}:J{row}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Thin : ExcelBorderStyle.Medium;
            sheet.Cells[$"H{row}:J{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"K{row}:L{row}"].Merge = true;
            sheet.Cells[$"K{row}:L{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"K{row}:L{row}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"K{row}:L{row}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Thin : ExcelBorderStyle.Medium;
            sheet.Cells[$"K{row}:L{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"K{row}:L{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        public void BackupRowStyle(ExcelWorksheet sheet, int row)
        {
            sheet.Rows[row].Height = 30;

            sheet.Cells[$"A{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Cells[$"B{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"B{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"B{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Cells[$"C{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Cells[$"D{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"D{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"D{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Cells[$"E{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"E{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"E{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        public double MeasureTextHeight(string text, ExcelFont font, double width)
        {
            if (text.IsNullOrEmpty())
                return 0.0;

            var bitmap = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(bitmap);
            var pixelWidth = Convert.ToInt32(width * 7); // 7 pixels per Excel column width
            var fontSize = font.Size * 1.01f;
            var drawingFont = new Font(font.Name, fontSize);
            var size = graphics.MeasureString(text, drawingFont, pixelWidth, new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces });

            // Convert to points (72 DPI and 96 points per inch) with a max of 409 (Excel requirement)
            return Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
        }

        public double MeasureTextHeightWithLineBreak(string text, ExcelFont font, double width)
        {
            if (text.IsNullOrEmpty())
                return 0.0;

            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var totalHeight = 0.0;

            foreach (var line in lines)
            {
                var bitmap = new Bitmap(1, 1);
                var graphics = Graphics.FromImage(bitmap);
                var pixelWidth = Convert.ToInt32(width * 7); // 7 pixels per Excel column width
                var fontSize = font.Size * 1.01f;
                var drawingFont = new Font(font.Name, fontSize);
                var size = graphics.MeasureString(line, drawingFont, pixelWidth, new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces });

                // Convert to points (72 DPI and 96 points per inch) with a max of 409 (Excel requirement)
                totalHeight += Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
            }

            return totalHeight;
        }
    }
}

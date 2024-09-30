using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml;
using System.Text;

namespace SupervisorMobility.API.DataAccess.Services.ExportationServices
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
            sheet.Cells[$"C{rownumber}:D{rownumber}"].Style.WrapText = true;
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
            sheet.Cells[$"F{rownumber}:G{rownumber}"].Style.WrapText = true;
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
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.WrapText = true;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        }

        public void ApplySequenceStyles(ExcelWorksheet sheet, int rownumber, bool last)
        {
            sheet.Rows[rownumber].Height = 20;

            sheet.Cells[$"B{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"B{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"B{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"B{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"B{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"C{rownumber}:G{rownumber}"].Merge = true;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.WrapText = true;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

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
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.WrapText = true;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Medium;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        }

        public void ApplyDistributionStyles(ExcelWorksheet sheet, int rownumber, bool firstPage)
        {
            var usedFont = sheet.Cells[$"B{rownumber - 1}"].Style.Font;
            sheet.Rows[rownumber].Height = 21.8;

            sheet.Cells[$"B{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            sheet.Cells[$"B{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"B{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
            sheet.Cells[$"B{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"B{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"C{rownumber}:G{rownumber}"].Merge = true;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.WrapText = true;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Merge = true;
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.WrapText = true;
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            
            sheet.Cells[$"L{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"L{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"L{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
            sheet.Cells[$"L{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"L{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sheet.Cells[$"M{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"M{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"M{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
            sheet.Cells[$"M{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"M{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sheet.Cells[$"N{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"N{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"N{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
            sheet.Cells[$"N{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"N{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            if (firstPage)
            {
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Merge = true;
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                
                sheet.Cells[$"Q{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"Q{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"Q{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
                sheet.Cells[$"Q{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"Q{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            else
            {
                sheet.Cells[$"O{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"O{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"O{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
                sheet.Cells[$"O{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"O{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells[$"P{rownumber}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"P{rownumber}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"P{rownumber}"].Style.Border.Bottom.Style = /*!last ? ExcelBorderStyle.Dashed :*/ ExcelBorderStyle.Thin;
                sheet.Cells[$"P{rownumber}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"P{rownumber}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            applyDistFonts(sheet,rownumber,usedFont,firstPage);
        }

        private void applyDistFonts(ExcelWorksheet sheet, int rownumber, ExcelFont usedFont, bool firstPage)
        {
            sheet.Cells[$"B{rownumber}"].Style.Font.Name = usedFont.Name;
            sheet.Cells[$"B{rownumber}"].Style.Font.Size = usedFont.Size;

            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Font.Name = usedFont.Name;
            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Font.Size = usedFont.Size;

            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.Font.Name = usedFont.Name;
            sheet.Cells[$"H{rownumber}:K{rownumber}"].Style.Font.Size = usedFont.Size;

            sheet.Cells[$"L{rownumber}"].Style.Font.Name = usedFont.Name;
            sheet.Cells[$"L{rownumber}"].Style.Font.Size = usedFont.Size;

            sheet.Cells[$"M{rownumber}"].Style.Font.Name = usedFont.Name;
            sheet.Cells[$"M{rownumber}"].Style.Font.Size = usedFont.Size;

            sheet.Cells[$"N{rownumber}"].Style.Font.Name = usedFont.Name;
            sheet.Cells[$"N{rownumber}"].Style.Font.Size = usedFont.Size;

            if (firstPage)
            {
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Style.Font.Name = usedFont.Name;
                sheet.Cells[$"O{rownumber}:P{rownumber}"].Style.Font.Size = usedFont.Size;

                sheet.Cells[$"Q{rownumber}"].Style.Font.Name = usedFont.Name;
                sheet.Cells[$"Q{rownumber}"].Style.Font.Size = usedFont.Size;
            }
            else
            {
                sheet.Cells[$"O{rownumber}"].Style.Font.Name = usedFont.Name;
                sheet.Cells[$"O{rownumber}"].Style.Font.Size = usedFont.Size;

                sheet.Cells[$"P{rownumber}"].Style.Font.Name = usedFont.Name;
                sheet.Cells[$"P{rownumber}"].Style.Font.Size = usedFont.Size;
            }
        }

        public void ChangeLastRowStyleAnalysis(ExcelWorksheet sheet, int rownumber, bool last)
        {
            sheet.Cells[$"B{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"C{rownumber}:D{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"E{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"F{rownumber}:G{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"H{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"I{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;
        }

        public void ChangeLastRowStyleSequence(ExcelWorksheet sheet, int rownumber, bool last)
        {
            sheet.Cells[$"B{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"C{rownumber}:G{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"H{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"I{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;

            sheet.Cells[$"J{rownumber}:L{rownumber}"].Style.Border.Bottom.Style = !last ? ExcelBorderStyle.Dashed : ExcelBorderStyle.Thin;
        }

        public void SetSpecialCasesFirstColumnStyle(ExcelWorksheet sheet, int initialrow, int lastrow)
        {
            sheet.Cells[$"B{initialrow}:D{lastrow}"].Merge = true;
            sheet.Cells[$"B{initialrow}:D{lastrow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            sheet.Cells[$"B{initialrow}:D{lastrow}"].Style.WrapText = true;
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

        public void SetAbnormalsAndImgsStyles(ExcelWorksheet worksheet, int abnormalStarts, int rowIndex, string currentChar)
        {
            SetSpecialCasesFirstColumnStyle(worksheet, abnormalStarts, rowIndex);

            int templateRow;

            switch (currentChar) //To check where in the template sheet is the row to start merging cells
            {
                case "A":
                    bool AnalOSequ = worksheet.Name.Contains("Analysis");
                    templateRow = AnalOSequ ? 13 : 15;

                    worksheet.Cells[$"M{templateRow}:P{rowIndex}"].Merge = true;
                    worksheet.Cells[$"M{templateRow}:P{rowIndex}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[$"M{templateRow}:P{rowIndex}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    break;
                default:
                    templateRow = 6;

                    worksheet.Cells[$"M{templateRow}:M{rowIndex}"].Merge = true;
                    worksheet.Cells[$"M{templateRow}:M{rowIndex}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[$"M{templateRow}:M{rowIndex}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    break;
            }
        }

        public void SetDistributionImgsStyles(ExcelWorksheet worksheet, int startingrow, int rowIndex, bool isFirst = false)
        {
            if (isFirst) //To check where in the template sheet is the row to start merging cells
            {
                worksheet.Cells[$"R{startingrow}:Z{rowIndex}"].Merge = true;
                worksheet.Cells[$"R{startingrow}:Z{rowIndex}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                //worksheet.Cells[$"M{startingrow}:P{rowIndex}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }
            else
            {
                worksheet.Cells[$"Q{startingrow}:W{rowIndex}"].Merge = true;
                worksheet.Cells[$"Q{startingrow}:W{rowIndex}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                //worksheet.Cells[$"M{startingrow}:M{rowIndex}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }
        }

        public void DistTimesNullStyle(ExcelWorksheet sheet, bool fp, int rowindex, ref char col)
        {
            ExcelRange cells;
            if (fp && col == 'O')
            {
                cells = sheet.Cells[$"{col}{rowindex}:P{rowindex}"];
                CrossedBorder(cells);
                col = 'P';
            }
            else
            {
                cells = sheet.Cells[$"{col}{rowindex}"];
                CrossedBorder(cells);
            }
        }

        public void CrossedBorder(ExcelRange cells)
        {
            cells.Style.Border.Diagonal.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.DiagonalUp = true;
        }

        public double CalculateRowHeight(string text, double columnWidth, double fontsize = 11, double lineSpacing = 1.2)
        {
            // Count the number of line breaks
            int lineBreaks = text.Split(new[] { '\n' }, StringSplitOptions.None).Length;

            // Estimate the number of lines the text will occupy
            double lineCount = (int)Math.Ceiling(text.Length / columnWidth) + lineBreaks;

            // Adjust the row height based on the estimated line count
            double lineHeight = fontsize * lineSpacing;
            return lineHeight * lineCount;
        }

        public (string fittingLines, string overflowText) SplitTextByRowHeight(string newText, double columnWidth, double fontsize = 11, double lineSpacing = 1.2, double maxRowHeight = 20, string existingText = "")
        {
            List<string> lines = new List<string>();
            string firstLine = "";
            double lineHeight = fontsize * lineSpacing;
            double currentHeight = 0;
            StringBuilder currentLine = new StringBuilder();
            StringBuilder overflowText = new StringBuilder();

            // Add existing text lines first
            if (!string.IsNullOrEmpty(existingText))
            {
                foreach (var line in existingText.Split(new[] { '\n' }, StringSplitOptions.None))
                {
                    lines.Add(line);
                    currentHeight += lineHeight;
                }

                firstLine = string.Join(" ", lines) + "\n";
                lines.Clear();
            }

            foreach (var word in newText.Split(' '))
            {
                // Estimate the width of the current line with the new word
                double estimatedLineWidth = (currentLine.Length + word.Length) * (fontsize * 0.6); // Approximate character width

                if (estimatedLineWidth > columnWidth)
                {
                    // Check if the current height exceeds the max row height
                    if (currentHeight + lineHeight > maxRowHeight)
                    {
                        overflowText.Append(currentLine.ToString() + " ");
                        currentLine.Clear();
                        overflowText.Append(word + " ");
                        continue;
                    }

                    // Add the current line to the list and reset
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    currentHeight += lineHeight;
                }

                if (currentLine.Length > 0)
                {
                    currentLine.Append(" ");
                }
                currentLine.Append(word);
            }

            // Add the last line if any
            if (currentLine.Length > 0)
            {
                // Check if adding the last line exceeds the max row height
                if (currentHeight + lineHeight > maxRowHeight)
                {
                    overflowText.Append(currentLine.ToString());
                }
                else
                {
                    lines.Add(currentLine.ToString());
                }
            }

            var joined = string.Join(" ", lines);

            return (string.Join("",firstLine,joined), overflowText.ToString().Trim());
        }
    }
}

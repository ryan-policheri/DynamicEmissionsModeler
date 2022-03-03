using System.Data;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace UIowaBuildingsServices
{
    public class ExcelBuilderBase : ExcelReaderBase
    {
        protected const int _headerFontSize = 13;
        protected const int _defaultFirstColumn = 1;

        public ExcelBuilderBase()
        {
        }

        protected int BuildDataSection(ExcelWorksheet worksheet, DataTable data, int startRow, bool includeHeaders = true)
        {
            string startingCell = "A" + startRow;
            worksheet.Cells[startingCell].LoadFromDataTable(data, includeHeaders);
            int endOfDataSection = includeHeaders ? startRow + data.Rows.Count : startRow + data.Rows.Count - 1;
            int columnCount = data.Columns.Count;

            TypeFormatColumns(worksheet, data);
            GridifySection(worksheet, startRow, _defaultFirstColumn, endOfDataSection, columnCount, ExcelBorderStyle.Thin, ExcelBorderStyle.Thin);
            if (includeHeaders)
            {
                ApplyHeaderFontStyle(worksheet, startRow, columnCount);
                AlternateRowHighlighting(worksheet, startRow + 1, endOfDataSection, columnCount, Color.Gainsboro);
            }
            else
            {
                AlternateRowHighlighting(worksheet, startRow, endOfDataSection, columnCount, Color.Gainsboro);
            }

            return endOfDataSection;
        }

        protected void AlternateRowHighlighting(ExcelWorksheet worksheet, int startRow, int endRow, int columnCount, Color alternateColor)
        {
            for (int i = startRow; i <= endRow; i++)
            {
                if ((startRow % 2 == 0 && i % 2 != 0) //if data starts on an even row and the current row is odd
                 || (startRow % 2 != 0 && i % 2 == 0)) //if data starts on an odd row and the current row is even
                {
                    var range = worksheet.Cells[i, _defaultFirstColumn, i, columnCount];
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(alternateColor);
                }
            }
        }

        protected void TypeFormatColumns(ExcelWorksheet worksheet, DataTable dataTable)
        {
            int colNumber = 0;
            foreach (DataColumn col in dataTable.Columns) //change date columns to date format
            {
                colNumber++;
                if (col.DataType == typeof(DateTime))
                {
                    worksheet.Column(colNumber).Style.Numberformat.Format = "MM/dd/yyyy";
                }
            }
        }

        protected void BuildWorksheetHeader(ExcelWorksheet worksheet, string text, int row, int columnCount)
        {
            string workSheetHeaderText = text;
            BuildSingleValueRow(worksheet, workSheetHeaderText, columnCount, row, Color.White);
            GridifySection(worksheet, row, _defaultFirstColumn, row, columnCount, ExcelBorderStyle.None, ExcelBorderStyle.Thin, false, false);
            ApplyHeaderFontStyle(worksheet, row, columnCount);
            worksheet.Cells[row, _defaultFirstColumn, row, columnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        protected void BuildSingleValueRow(ExcelWorksheet worksheet, string text, int columnCount, int rowToBuildOn, Color color)
        {
            var range = worksheet.Cells[rowToBuildOn, _defaultFirstColumn, rowToBuildOn, columnCount];
            range.Merge = true;
            range.Value = text;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color);
        }

        protected void BuildMultiValueRow(ExcelWorksheet worksheet, IEnumerable<string> values, int rowToBuildOn, Color color)
        {
            int column = 1;
            foreach (string value in values)
            {
                var cell = worksheet.Cells[rowToBuildOn, column++];
                cell.Value = value;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(color);
            }
        }

        protected void ApplyHeaderFontStyle(ExcelWorksheet worksheet, int row, int columnCount)
        {
            BoldenRange(worksheet, row, columnCount);
            var range = worksheet.Cells[row, _defaultFirstColumn, row, columnCount];
            range.Style.Font.Size = _headerFontSize;
        }

        protected void BoldenRange(ExcelWorksheet worksheet, int row, int columnCount)
        {
            var range = worksheet.Cells[row, _defaultFirstColumn, row, columnCount];
            range.Style.Font.Bold = true;
        }

        protected void AlignCell(ExcelWorksheet worksheet, int row, int column, ExcelHorizontalAlignment horizontalAlignment = ExcelHorizontalAlignment.Left, ExcelVerticalAlignment verticalAlignment = ExcelVerticalAlignment.Bottom)
        {
            var range = worksheet.Cells[row, column, row, column];
            range.Style.HorizontalAlignment = horizontalAlignment;
            range.Style.VerticalAlignment = verticalAlignment;
        }

        protected void GridifySection(ExcelWorksheet worksheet, int fromRow, int fromColumn, int toRow, int toColumn,
            ExcelBorderStyle horizontalBorderStyle, ExcelBorderStyle verticalBorderStyle, bool thickOuterBottom = false, bool thickOuterTop = false, bool ensureOuterSides = false)
        {
            var range = worksheet.Cells[fromRow, fromColumn, toRow, toColumn];
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            range.Style.Border.Top.Style = horizontalBorderStyle;
            range.Style.Border.Left.Style = verticalBorderStyle;
            range.Style.Border.Right.Style = verticalBorderStyle;
            range.Style.Border.Bottom.Style = horizontalBorderStyle;

            if (thickOuterBottom) worksheet.Cells[toRow, fromColumn, toRow, toColumn].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            if (thickOuterTop) worksheet.Cells[fromRow, fromColumn, fromRow, toColumn].Style.Border.Top.Style = ExcelBorderStyle.Thick;
            if (ensureOuterSides)
            {
                worksheet.Cells[fromRow, fromColumn, toRow, fromColumn].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[fromRow, toColumn, toRow, toColumn].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
        }

        protected void FillRangeBackground(ExcelWorksheet worksheet, int fromRow, int fromColumn, int toRow, int toColumn, Color color)
        {
            var range = worksheet.Cells[fromRow, fromColumn, toRow, toColumn];
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color);
        }

        protected void FreezeRowAndAbove(ExcelWorksheet worksheet, int row)
        {
            worksheet.View.FreezePanes(++row, _defaultFirstColumn); //freezes all the cells before the given dimensions (not inclusive)
        }

        protected void AutoFitColumns(ExcelWorksheet worksheet)
        {
            worksheet.Cells.AutoFitColumns();
        }
    }
}
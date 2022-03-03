using DotNetCommon.Extensions;
using OfficeOpenXml;

namespace UIowaBuildingsServices
{
    public class ExcelReaderBase
    {
        private const int _defaultHeaderRow = 1;

        internal IEnumerable<T> ExtractObjectsFromWorksheet<T>(ExcelWorksheet worksheet) where T : new()
        {
            ICollection<T> objects = new List<T>();
            if (worksheet.Dimension.Rows < 2) return objects;

            string[] headerProperties = ParseAvailablePropertiesFromHeader<T>(worksheet); //Assume row 1 header
            if (headerProperties.Count() < 1) return objects;

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++) //Assume row 2 is first data row
            {
                T obj = new T();

                for (int column = 1; column <= headerProperties.Count(); column++)
                {
                    string cellValue = GetCellValue(worksheet, row, column);
                    string propertyName = headerProperties[column - 1];
                    if (!String.IsNullOrWhiteSpace(cellValue) && !String.IsNullOrWhiteSpace(propertyName))
                    {
                        typeof(T).GetProperty(propertyName).SetValueWithTypeRespect(obj, cellValue);
                    }
                }

                objects.Add(obj);
            }

            return objects;
        }

        internal string[] ParseAvailablePropertiesFromHeader<T>(ExcelWorksheet worksheet)
        {
            ICollection<string> propertyIndexList = new List<string>();

            int columnIndex = 1;
            while (!String.IsNullOrWhiteSpace(GetCellValue(worksheet, _defaultHeaderRow, columnIndex)))
            {
                string headerValue = GetCellValue(worksheet, _defaultHeaderRow, columnIndex);
                if (typeof(T).HasCloseMatchingProperty(headerValue))
                {
                    propertyIndexList.Add(typeof(T).GetCloseMatchPropertyName(headerValue));
                }
                else { propertyIndexList.Add(null); }

                columnIndex++;
            }

            return propertyIndexList.ToArray();
        }

        internal string GetCellValue(ExcelWorksheet worksheet, int rowIndex, int columnIndex) //Excel indexes are 1-based
        {
            return worksheet.Cells[rowIndex, columnIndex]?.Value?.ToString();
        }
    }
}

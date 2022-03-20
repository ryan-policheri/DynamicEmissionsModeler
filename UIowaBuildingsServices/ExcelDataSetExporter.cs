using System.Data;
using OfficeOpenXml;

namespace UIowaBuildingsServices
{
    public class ExcelDataSetExporter : ExcelBuilderBase
    {
        public ExcelDataSetExporter()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public void ExportDataSetToExcel(string filePath, DataTable dataTable, string header)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Excel Export");
                this.BuildWorksheetHeader(worksheet, header, 1, dataTable.Columns.Count);
                this.BuildDataSection(worksheet, dataTable, 2);
                this.AutoFitColumns(worksheet);

                FileInfo file = new FileInfo(filePath);
                package.SaveAs(file);
            }
        }
    }
}
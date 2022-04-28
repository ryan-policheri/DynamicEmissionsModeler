using System.Data;
using DotNetCommon.Extensions;
using OfficeOpenXml;
using UIowaBuildingsModel;

namespace UIowaBuildingsServices
{
    public class ExcelExportService : ExcelBuilderBase
    {
        public ExcelExportService()
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

        public void ExportCampusSnapshot(string filePath, CampusSnapshot snapshot)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet buildingEmissionsSheet = package.Workbook.Worksheets.Add("Building Emissions");
                int row = 1;
                foreach (BuildingUsageSummary summary in snapshot.BuildingUsageSummaries)
                {
                    row = CreateTable(buildingEmissionsSheet, BuildHeaderString(summary.BuildingName + " Emissions", snapshot), row, summary.BuildDataTable()); 
                }

                ExcelWorksheet campusEnergyData = package.Workbook.Worksheets.Add("Campus Energy Summary");
                row = 1; CreateTable(campusEnergyData, "Campus Energy Resources Summary", row, snapshot.BuildEnergyResourcesTable());

                foreach(var mapper in snapshot.CampusDataSources.ChilledWaterMapper.PlantMappers)
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets.Add(mapper.Name);
                    row = 1; CreateTable(sheet, mapper.Name, row, mapper.BuildDataTable(snapshot.Start, snapshot.End));
                }

                ExcelWorksheet gridSummary = package.Workbook.Worksheets.Add("Electric Grid Summary");
                row = 1; CreateTable(gridSummary, $"{snapshot.ElectricGridStrategy}", row, snapshot.BuildGridElectricTable());

                FileInfo file = new FileInfo(filePath);
                package.SaveAs(file);
            }
        }

        private int CreateTable(ExcelWorksheet sheet, string header, int row, DataTable table)
        {
            this.BuildWorksheetHeader(sheet, header, row, table.Columns.Count);
            row = this.BuildDataSection(sheet, table, ++row);
            this.AutoFitColumns(sheet);
            return row + 2;
        }

        private string BuildHeaderString(string title, CampusSnapshot snapshot)
        {
            string header = $"{title} - " + snapshot.Start.LocalDateTime.Date.ToShortDateString();
            if (snapshot.Start.EnumerateHoursUntil(snapshot.End).Count() > 24) header += " - " + snapshot.End.LocalDateTime.Date.ToShortTimeString();
            return header;
        }
    }
}
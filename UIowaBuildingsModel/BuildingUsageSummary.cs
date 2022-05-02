using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIowaBuildingsModel
{
    public class BuildingUsageSummary
    {
        public string BuildingName { get; set; }
        public double SquareFeet { get; set; }
        public IEnumerable<BuildingUsage> BuildingUsages { get; set; }

        public DataTable BuildDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Hour");
            table.Columns.Add("Electric Usage (kwh)");
            table.Columns.Add("Steam Usage (mmbtu)");
            table.Columns.Add("Chilled Water Usage (gallons)");
            table.Columns.Add("CO2 Emissions from Electric (kg)");
            table.Columns.Add("CO2 Emissions from Steam (kg)");
            table.Columns.Add("CO2 Emissions from Chilled Water (kg)");
            table.Columns.Add("Total CO2 Emissions (kg)");
            table.Columns.Add("Total CO2 Emissions (gasoline gallons)");
            table.Columns.Add("CO2 Emissions per Square Foot (gasoline gallons / sq. ft)");

            foreach (BuildingUsage usage in BuildingUsages.OrderBy(x => x.Timestamp))
            {
                DataRow row = table.NewRow();
                row["Hour"] = usage.Timestamp.LocalDateTime.ToString();
                row["Electric Usage (kwh)"] = usage.ElectricUsage.KilowattHours;
                row["Steam Usage (mmbtu)"] = usage.SteamUsageAsEnergy.MegabritishThermalUnits;
                row["Chilled Water Usage (gallons)"] = usage.ChilledWaterUsage.UsGallons;
                row["CO2 Emissions from Electric (kg)"] = usage.Co2FromElectricUsage.Kilograms;
                row["CO2 Emissions from Steam (kg)"] = usage.Co2FromSteamUsage.Kilograms;
                row["CO2 Emissions from Chilled Water (kg)"] = usage.Co2FromChilledWaterUsage.Kilograms;
                row["Total CO2 Emissions (kg)"] = usage.TotalCo2.Kilograms;
                row["Total CO2 Emissions (gasoline gallons)"] = usage.TotalCo2InGasolineVolumeEquivelent.UsGallons;
                row["CO2 Emissions per Square Foot (gasoline gallons / sq. ft)"] = usage.TotalCo2InGasolineVolumeEquivelent.UsGallons / this.SquareFeet;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}

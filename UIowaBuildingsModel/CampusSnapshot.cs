using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIowaBuildingsModel
{
    public class CampusSnapshot
    {
        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        public IEnumerable<BuildingUsageSummary> BuildingUsageSummaries { get; set; }

        public IEnumerable<HourDetails> EnergyResources { get; set; }
        
        public PowerPlantDataMapper CampusDataSources { get; set; }

        public DataTable BuildEnergyResourcesTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Hour");
            table.Columns.Add("Grid Electric (mwh)");
            table.Columns.Add("Cogenerated Electric (mwh)");
            table.Columns.Add("Steam Produced (mmbtu)");
            table.Columns.Add("Chilled Water Produced (gallons)");
            table.Columns.Add("Power Plant Electric Overhead (mwh)");
            table.Columns.Add("Power Plant Steam Overhead (mmbtu)");
            table.Columns.Add("Electric Emissions Factor (kg-co2/mwh)");
            table.Columns.Add("Steam Emissions Factor (kg-co2/mmbtu)");
            table.Columns.Add("Chilled Water Emissions Factor (kg-co2/mmbtu)");

            foreach (HourDetails details in EnergyResources.OrderBy(x => x.Hour))
            {
                DataRow row = table.NewRow();
                row["Hour"] = details.Hour.LocalDateTime.ToString();
                row["Grid Electric (mwh)"] = details.ElectricPurchased.MegawattHours;
                row["Cogenerated Electric (mwh)"] = details.CogeneratedElectric.MegawattHours;
                row["Steam Produced (mmbtu)"] = details.SteamProduced.MegabritishThermalUnits;
                row["Chilled Water Produced (gallons)"] = details.ChilledWaterProduced.UsGallons;
                row["Power Plant Electric Overhead (mwh)"] = details.PowerPlantElectricOverhead.MegawattHours;
                row["Power Plant Steam Overhead (mmbtu)"] = details.PowerPlantSteamOverhead.MegabritishThermalUnits;
                row["Electric Emissions Factor (kg-co2/mwh)"] = details.ElectricEmissionsFactorInKilogramsOfCo2PerMegawattHour;
                row["Steam Emissions Factor (kg-co2/mmbtu)"] = details.SteamEmissionsFactorInKilogramsOfCo2PerMmbtu;
                row["Chilled Water Emissions Factor (kg-co2/mmbtu)"] = details.ChilledWaterEmissionsFactorInKilogramsOfCo2PerChilledWaterGallon;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}

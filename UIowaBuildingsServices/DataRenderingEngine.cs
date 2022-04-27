using System.Data;
using DotNetCommon.Extensions;
using UIowaBuildingsModel;

namespace UIowaBuildingsServices
{
    public class DataRenderingEngine
    {
        private readonly IEnumerable<DateTimeOffset> _hours;
        private readonly CampusEnergyResourceManager _manager;
        private readonly ICollection<BuildingUsageMapper> _buildings;

        public DataRenderingEngine(CampusEnergyResourceManager reporter, ICollection<BuildingUsageMapper> buildings)
        {
            _manager = reporter;
            _buildings = buildings;

            _hours = _manager.EnumerateHours();
            foreach (BuildingUsageMapper building in _buildings)
            {
                bool allHoursMatch = _hours.AllHoursMatch(building.EnumerateDataHours());
                if (!allHoursMatch) throw new ArgumentException("Arguments have a mismatch in the timeframe they represent");
            }
        }

        public TableWithMeta BuildDailySummary()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Building Name");
            table.Columns.Add("Daily Kilowatt Hours");
            table.Columns.Add("Daily CO2 Emissions in Kilograms");

            foreach (BuildingUsageMapper building in _buildings)
            {
                DataRow row = table.NewRow();
                row["Building Name"] = building.BuildingName;

                double totalDemandInkwh = 0;
                double dailyCo2EmissionsInKg = 0;
                foreach (BuildingUsage usage in building.PackageDataIntoBuildingUsageSummary().BuildingUsages.OrderBy(x => x.Timestamp))
                {
                    double hourlyDemandInKiloWattHours = usage.ElectricUsage.KilowattHours;
                    double hourlyEmissionsInKilograms = _manager.GetHour(usage.Timestamp).CalculateCo2EmissionsFromElectricUsage(usage.ElectricUsage).Kilograms;
                    totalDemandInkwh += hourlyDemandInKiloWattHours;
                    dailyCo2EmissionsInKg += hourlyEmissionsInKilograms;
                }

                row["Daily Kilowatt Hours"] = totalDemandInkwh / (_hours.Count() / 24);
                row["Daily CO2 Emissions in Kilograms"] = dailyCo2EmissionsInKg / (_hours.Count() / 24);

                table.Rows.Add(row);
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Daily Emissions Report: " + _manager.StartDateTime.LocalDateTime.Date.ToShortDateString();
            if (_hours.Count() > 24) tableWithMeta.Header += " - " + _manager.EndDateTime.LocalDateTime.Date.ToShortTimeString();

            return tableWithMeta;
        }

        public TableWithMeta BuildHourlySummary()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Building Name");
            table.Columns.Add("Hour");
            table.Columns.Add("Electric Usage (kwh)");
            table.Columns.Add("Steam Usage (mmbtu)");
            table.Columns.Add("Chilled Water Usage (gallons)");
            table.Columns.Add("CO2 Emissions in Kilograms");

            foreach (BuildingUsageMapper building in _buildings)
            {
                foreach (BuildingUsage usage in building.PackageDataIntoBuildingUsageSummary().BuildingUsages.OrderBy(x => x.Timestamp))
                {
                    DataRow row = table.NewRow();
                    row["Building Name"] = building.BuildingName;
                    row["Hour"] = usage.Timestamp.LocalDateTime.ToShortDateString() + " Hour: " + usage.Timestamp.LocalDateTime.Hour;
                    row["Electric Usage (kwh)"] = usage.ElectricUsage.KilowattHours;
                    row["Steam Usage (mmbtu)"] = usage.SteamUsageAsEnergy.MegabritishThermalUnits;
                    row["Chilled Water Usage (gallons)"] = usage.ChilledWaterUsage.UsGallons;
                    row["CO2 Emissions in Kilograms"] = _manager.GetHour(usage.Timestamp).CalculateCo2EmissionsFromElectricUsage(usage.ElectricUsage).Kilograms;
                    table.Rows.Add(row);
                }
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Hourly Emissions Report - " + _manager.StartDateTime.LocalDateTime.Date.ToShortDateString();
            if (_hours.Count() > 24) tableWithMeta.Header += " - " + _manager.EndDateTime.LocalDateTime.Date.ToShortTimeString();

            return tableWithMeta;
        }

        public TableWithMeta BuildMisoEmissionsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Hour");

            bool sourceHeadersAdded = false;

            foreach (DateTimeOffset hour in _hours)
            {
                HourDetails hourDetails = _manager.GetHour(hour);
                IEnumerable<ElectricGridSource> gridSources = hourDetails.EnumerateElectricGridSources();
                DataRow row = table.NewRow();

                foreach (ElectricGridSource gridSource in gridSources)
                {
                    if (!sourceHeadersAdded)
                    {
                        table.Columns.Add("Megawatt hours from " + gridSource.SourceName);
                        table.Columns.Add("CO2 in Kilograms from " + gridSource.SourceName);
                    }

                    row["Hour"] = hour.LocalDateTime.ToString();
                    row["Megawatt hours from " + gridSource.SourceName] = gridSource.ElectricEnergyFromSource.MegawattHours;
                    row["CO2 in Kilograms from " + gridSource.SourceName] = gridSource.Co2FromSource.Kilograms;
                }

                table.Rows.Add(row);
                sourceHeadersAdded = true;
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Miso Emissions Report - " + _manager.StartDateTime.LocalDateTime.Date.ToShortDateString();
            if (_hours.Count() > 24) tableWithMeta.Header += " - " + _manager.EndDateTime.LocalDateTime.Date.ToShortTimeString();

            return tableWithMeta;
        }

        public TableWithMeta BuildSourceCoefficientsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Source");
            table.Columns.Add("Kilograms of CO2 per kilowatt hour");

            IDictionary<string, double> kilogramsOfCo2BySourceName = new Dictionary<string, double>();
            IDictionary<string, double> kilowattHoursOfEnergyBySourceName = new Dictionary<string, double>();

            foreach (HourDetails details in _manager.EnumerateHoursDetails())
            {
                foreach(ElectricGridSource gridSource in details.EnumerateElectricGridSources())
                {
                    if(!kilogramsOfCo2BySourceName.ContainsKey(gridSource.SourceName)) kilogramsOfCo2BySourceName[gridSource.SourceName] = 0;
                    if(!kilowattHoursOfEnergyBySourceName.ContainsKey(gridSource.SourceName)) kilowattHoursOfEnergyBySourceName[gridSource.SourceName] = 0;

                    kilogramsOfCo2BySourceName[gridSource.SourceName] += gridSource.Co2FromSource.Kilograms;
                    kilowattHoursOfEnergyBySourceName[gridSource.SourceName] += gridSource.ElectricEnergyFromSource.KilowattHours;
                }
            }

            foreach(string key in kilogramsOfCo2BySourceName.Keys)
            {
                DataRow row = table.NewRow();
                row["Source"] = key;
                row["Kilograms of CO2 per kilowatt hour"] = kilogramsOfCo2BySourceName[key] / kilowattHoursOfEnergyBySourceName[key];
                table.Rows.Add(row);
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Source Coefficients";

            return tableWithMeta;
        }
    }
}
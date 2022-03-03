using System.Data;
using DotNetCommon.Extensions;
using PiModel;
using UIowaBuildingsModel;

namespace UIowaBuildingsServices
{
    internal class DataRenderingEngine
    {
        private ICollection<Asset> _assets;
        private IEnumerable<HourSummary> _summaries;

        public DataRenderingEngine(ICollection<Asset> assets, IEnumerable<HourSummary> summaries)
        {
            _assets = assets;
            _summaries = summaries;
        }

        public TableWithMeta BuildDailySummary()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Building Name");
            table.Columns.Add("Daily Kilowatt Hours");
            table.Columns.Add("Daily CO2 Emissions in Kilograms");

            foreach (Asset asset in _assets)
            {
                DataRow row = table.NewRow();
                row["Building Name"] = asset.Name;
                Value dailyKwh = asset.ChildValues.First(x => x.Name.CapsAndTrim() == "EL DAILY KWH").Value;
                double asDouble = double.Parse(dailyKwh.UntypedValue.ToString());
                row["Daily Kilowatt Hours"] = asDouble;

                double dailyCo2EmissionsInKg = 0;
                double crudeHourlyDemand = asDouble / 24;
                foreach (HourSummary summary in _summaries)
                {
                    double hourlyEmissions = summary.CalculateHourlyCO2EmissionsInKg(crudeHourlyDemand);
                    dailyCo2EmissionsInKg += hourlyEmissions;
                }

                row["Daily CO2 Emissions in Kilograms"] = dailyCo2EmissionsInKg;

                table.Rows.Add(row);
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Daily Emissions Report - " + _summaries.OrderByDescending(x => x.Hour).First().Hour.ToShortDateString();

            return tableWithMeta;
        }

        public TableWithMeta BuildHourlySummary()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Building Name");
            table.Columns.Add("Hour");
            table.Columns.Add("Kilowatt Hours");
            table.Columns.Add("CO2 Emissions in Kilograms");

            foreach (Asset asset in _assets)
            {
                Value dailyKwh = asset.ChildValues.First(x => x.Name.CapsAndTrim() == "EL DAILY KWH").Value;
                double asDouble = double.Parse(dailyKwh.UntypedValue.ToString());

                double crudeHourlyDemand = asDouble / 24;
                foreach (HourSummary summary in _summaries)
                {
                    DataRow row = table.NewRow();
                    row["Building Name"] = asset.Name;
                    row["Hour"] = summary.Hour.ToShortDateString() + " Hour: " + summary.Hour.Hour;
                    row["Kilowatt Hours"] = crudeHourlyDemand;
                    double hourlyEmissions = summary.CalculateHourlyCO2EmissionsInKg(crudeHourlyDemand);
                    row["CO2 Emissions in Kilograms"] = hourlyEmissions;
                    table.Rows.Add(row);
                }
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Hourly Emissions Report - " + _summaries.OrderByDescending(x => x.Hour).First().Hour.ToShortDateString();

            return tableWithMeta;
        }

        public TableWithMeta BuildMisoEmissionsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Hour");

            foreach (Source source in _summaries.First().Sources.Select(x => x.Source))
            {
                table.Columns.Add("Megawatt hours from " + source.SourceName);
                table.Columns.Add("CO2 in Kilograms from " + source.SourceName);
            }

            foreach (HourSummary summary in _summaries)
            {
                DataRow row = table.NewRow();
                row["Hour"] = summary.Hour.ToShortDateString() + " Hour: " + summary.Hour.Hour;
                foreach (HourlySource source in summary.Sources)
                {
                    row["Megawatt hours from " + source.Source.SourceName] = source.Value;
                    row["CO2 in Kilograms from " + source.Source.SourceName] = summary.CalculateCO2EmissionsInKgFromSource(source, source.Value * 1000); //value is in mwh
                }
                table.Rows.Add(row);
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Miso Emissions Report - " + _summaries.OrderByDescending(x => x.Hour).First().Hour.ToShortDateString();

            return tableWithMeta;
        }

        public TableWithMeta BuildSourceCoefficientsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Source");
            table.Columns.Add("Kilograms of CO2 per kilowatt hour");

            foreach (Source source in _summaries.First().Sources.Select(x => x.Source))
            {
                DataRow row = table.NewRow();
                row["Source"] = source.SourceName;
                row["Kilograms of CO2 per kilowatt hour"] = source.KiloGramsOfCo2PerKwh;
                table.Rows.Add(row);
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Source Coefficients";

            return tableWithMeta;
        }
    }
}
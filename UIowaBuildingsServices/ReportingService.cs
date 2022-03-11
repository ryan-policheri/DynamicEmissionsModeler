using System.Data;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using EIA.Services.Clients;
using EIA.Domain.Model;
using PiServices;
using PiModel;
using UIowaBuildingsModel;
using DotNetCommon.Extensions;

namespace UIowaBuildingsServices
{
    public class ReportingService : ExcelBuilderBase
    {
        private readonly PiHttpClient _piClient;
        private readonly EiaClient _eiaClient;

        public ReportingService(PiHttpClient piClient, EiaClient eiaClient, ILogger<ReportingService> logger)
        {
            _piClient = piClient;
            _eiaClient = eiaClient;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task GenerateHourlyEmissionsReport(HourlyEmissionsReportParameters parameters, string outputFilePath)
        {
            //For every asset, get the electric power hourly average, where hours are in local time of this computing system
            ICollection<Asset> assets = new List<Asset>();
            foreach (string link in parameters.AssetLinks)
            {
                Asset asset = await _piClient.GetByDirectLink<Asset>(link);
                await _piClient.LoadAssetValueList(asset);
                AssetValue dailyElectric = asset.ChildValues.Where(x => x.Name.CapsAndTrim() == "EL POWER HOURLY AVG").First();
                asset.ChildValues.Remove(dailyElectric);
                dailyElectric = await _piClient.LoadAssetValueDetail(dailyElectric);
                await _piClient.LoadInterpolatedValues(dailyElectric, parameters.StartDateInLocalTime, parameters.EndDateInLocalTime);
                asset.ChildValues.Add(dailyElectric);
                assets.Add(asset);
            }

            //Pull the MISO data for every source for the given timeframe, in local time of this computing system
            IEnumerable<HourSummary> summaries = await BuildHourlySources(parameters);

            //These are the hours we care about for this report. We should have data from EIA and PI for all these hours
            IEnumerable<DateTime> hours = GenerateHoursInLocalTime(parameters.StartDateInLocalTime, parameters.EndDateInLocalTime);

            DataRenderingEngine engine = new DataRenderingEngine(hours, assets, summaries);

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet dailyEmissionsSummary = package.Workbook.Worksheets.Add("Daily Emissions Summary");
                TableWithMeta dailyEmissionsTable = engine.BuildDailySummary();
                this.BuildWorksheetHeader(dailyEmissionsSummary, dailyEmissionsTable.Header, 1, dailyEmissionsTable.ColumnCount);
                this.BuildDataSection(dailyEmissionsSummary, dailyEmissionsTable.Table, 2);
                this.AutoFitColumns(dailyEmissionsSummary);

                ExcelWorksheet hourlyEmissionsSummary = package.Workbook.Worksheets.Add("Hourly Emissions Summary");
                TableWithMeta hourlyEmissionsTable = engine.BuildHourlySummary();
                this.BuildWorksheetHeader(hourlyEmissionsSummary, hourlyEmissionsTable.Header, 1, hourlyEmissionsTable.ColumnCount);
                this.BuildDataSection(hourlyEmissionsSummary, hourlyEmissionsTable.Table, 2);
                this.AutoFitColumns(hourlyEmissionsSummary);


                ExcelWorksheet misoSummary = package.Workbook.Worksheets.Add("MISO Emissions Summary");
                TableWithMeta misoSummaryTable = engine.BuildMisoEmissionsTable();
                this.BuildWorksheetHeader(misoSummary, misoSummaryTable.Header, 1, misoSummaryTable.ColumnCount);
                this.BuildDataSection(misoSummary, misoSummaryTable.Table, 2);
                this.AutoFitColumns(misoSummary);

                ExcelWorksheet sourceCoefficients = package.Workbook.Worksheets.Add("Source Coefficients");
                TableWithMeta sourceCoefficientsTable = engine.BuildSourceCoefficientsTable();
                this.BuildWorksheetHeader(sourceCoefficients, sourceCoefficientsTable.Header, 1, sourceCoefficientsTable.ColumnCount);
                this.BuildDataSection(sourceCoefficients, sourceCoefficientsTable.Table, 2);
                this.AutoFitColumns(sourceCoefficients);

                FileInfo file = new FileInfo(outputFilePath);
                package.SaveAs(file);
            }
        }

        private async Task<IEnumerable<HourSummary>> BuildHourlySources(HourlyEmissionsReportParameters parameters)
        {
            int numberOfHours = 24;
            IEnumerable<Source> sources = BuildSources();

            ICollection<Series> seriesBySource = new List<Series>();
            foreach (Source source in sources)
            {//The sources are pulling everything in UTC time. There is an option to pull in local time but intentionally not using it
             //because it is the local time of the grid operator not of this computing system.
                Series sourceSeries = await _eiaClient.GetHourlySeriesByIdAsync(source.HourlySourceId, parameters.StartDateInLocalTime, parameters.EndDateInLocalTime, source.HourlySourceDateTimeKind);
                seriesBySource.Add(sourceSeries);
            }

            ICollection<HourSummary> summaries = new List<HourSummary>();
            DateTime hourIterator = parameters.StartDateInLocalTime.Date;

            for (int i = 0; i < numberOfHours; i++)
            {
                HourSummary summary = new HourSummary();
                summary.Hour = hourIterator;

                for (int j = 0; j < sources.Count(); j++)
                {
                    Source source = sources.ElementAt(j);
                    Series series = seriesBySource.ElementAt(j);
                    SeriesDataPoint seriesData = series.DataPoints.Where(x => x.Timestamp.Hour == hourIterator.Hour).First();
                    HourlySource hourlySource = source.ProduceHour(seriesData.Timestamp, seriesData.Value.Value);
                    summary.Sources.Add(hourlySource);
                }

                summaries.Add(summary);
                hourIterator = hourIterator.AddHours(1);
            }

            return summaries;
        }

        private IEnumerable<Source> BuildSources()
        {//https://www.eia.gov/tools/faqs/faq.php?id=74&t=11
            //Converted to kg because we like the metric system
            ICollection<Source> sources = new List<Source>();
            sources.Add(new Source { SourceName = "Wind", HourlySourceId = "EBA.MISO-ALL.NG.WND.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Solar", HourlySourceId = "EBA.MISO-ALL.NG.SUN.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Hydro", HourlySourceId = "EBA.MISO-ALL.NG.WAT.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Coal", HourlySourceId = "EBA.MISO-ALL.NG.COL.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 1.011511 });
            sources.Add(new Source { SourceName = "Natural Gas", HourlySourceId = "EBA.MISO-ALL.NG.NG.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 0.4127691 });
            sources.Add(new Source { SourceName = "Nuclear", HourlySourceId = "EBA.MISO-ALL.NG.NUC.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Petro", HourlySourceId = "EBA.MISO-ALL.NG.OIL.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 0.9661517 });
            sources.Add(new Source { SourceName = "Other", HourlySourceId = "EBA.MISO-ALL.NG.OTH.H", HourlySourceDateTimeKind = DateTimeKind.Utc, KiloGramsOfCo2PerKwh = 0.30 }); //What to use for other?
            return sources;
        }

        private IEnumerable<DateTime> GenerateHoursInLocalTime(DateTime startDate, DateTime endDate)
        {
            ICollection<DateTime> hours = new List<DateTime>();
            DateTime dateIterator = startDate.Date; //Should get 12AM on the starting date (first hour of day)
            while(dateIterator.Date <= endDate.Date)
            {
                hours.Add(dateIterator);
                dateIterator = dateIterator.AddHours(1);
            }
            return hours;
        }
    }
}
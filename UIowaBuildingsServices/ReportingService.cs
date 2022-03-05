using System.Globalization;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using EIA.Services.Clients;
using EIA.Domain.Model;
using PiServices;
using PiModel;
using UIowaBuildingsModel;
using System.Data;

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

        public async Task GenerateHourlyEmissionsReport(string outputFilePath)
        {
            ICollection<Asset> assets = new List<Asset>();

            string macleanLink = "https://pi.facilities.uiowa.edu/piwebapi/elements/F1EmAVYciAZHVU6DzQbJjxTxWwcE7mI49J6RGuHFS_ZKR9xgSVRTTlQyMjU5XFVJLUVORVJHWVxNQUlOIENBTVBVU1xNQUNMRUFOIEhBTEw";
            Asset maclean = await _piClient.GetByDirectLink<Asset>(macleanLink);
            await _piClient.LoadAssetValueList(maclean);
            assets.Add(maclean);

            string libraryLink = "https://pi.facilities.uiowa.edu/piwebapi/elements/F1EmAVYciAZHVU6DzQbJjxTxWw3k7mI49J6RGuHFS_ZKR9xgSVRTTlQyMjU5XFVJLUVORVJHWVxNQUlOIENBTVBVU1xNQUlOIExJQlJBUlk";
            Asset library = await _piClient.GetByDirectLink<Asset>(libraryLink);
            await _piClient.LoadAssetValueList(library);
            assets.Add(library);

            string usbLink = "https://pi.facilities.uiowa.edu/piwebapi/elements/F1EmAVYciAZHVU6DzQbJjxTxWwWFPfKY9J6RGuHFS_ZKR9xgSVRTTlQyMjU5XFVJLUVORVJHWVxNQUlOIENBTVBVU1xVTklWRVJTSVRZIFNFUlZJQ0VTIEJVSUxESU5H";
            Asset usb = await _piClient.GetByDirectLink<Asset>(usbLink);
            await _piClient.LoadAssetValueList(usb);
            assets.Add(usb);

            IEnumerable<HourSummary> summaries = await BuildHourlySources();

            DataRenderingEngine engine = new DataRenderingEngine(assets, summaries);

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

        private async Task<IEnumerable<HourSummary>> BuildHourlySources()
        {
            int numberOfHours = 24;
            IEnumerable<Source> sources = BuildSources();

            ICollection<Series> seriesBySource = new List<Series>();
            foreach (Source source in sources)
            {
                Series sourceSeries = await _eiaClient.GetSeriesByIdAsync(source.HourlySourceId);
                seriesBySource.Add(sourceSeries);
            }

            ICollection<HourSummary> summaries = new List<HourSummary>();
            DateTime startingHour = ParseHour(seriesBySource.First().Data.OrderByDescending(x => ParseHour(x.ColumnHeader)).First().ColumnHeader);

            for (int i = 0; i < numberOfHours; i++)
            {
                HourSummary summary = new HourSummary();
                summary.Hour = startingHour;

                for (int j = 0; j < sources.Count(); j++)
                {
                    Source source = sources.ElementAt(j);
                    Series series = seriesBySource.ElementAt(j);
                    SeriesData seriesData = series.Data.Where(x => ParseHour(x.ColumnHeader).Hour == startingHour.Hour).First();
                    HourlySource hourlySource = source.ProduceHour(ParseHour(seriesData.ColumnHeader), seriesData.ColumnValue.Value);
                    summary.Sources.Add(hourlySource);
                }

                summaries.Add(summary);
                startingHour = startingHour.AddHours(-1);
            }

            return summaries;
        }

        private DateTime ParseHour(string hourAsString)
        {
            CultureInfo us = new CultureInfo("en-US");
            return DateTime.ParseExact(hourAsString, "yyyyMMddTHH-mm", us);
        }

        private IEnumerable<Source> BuildSources()
        {//https://www.eia.gov/tools/faqs/faq.php?id=74&t=11
            //Converted to kg because we like the metric system
            ICollection<Source> sources = new List<Source>();
            sources.Add(new Source { SourceName = "Wind", HourlySourceId = "EBA.MISO-ALL.NG.WND.HL", KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Solar", HourlySourceId = "EBA.MISO-ALL.NG.SUN.HL", KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Hydro", HourlySourceId = "EBA.MISO-ALL.NG.WAT.HL", KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Coal", HourlySourceId = "EBA.MISO-ALL.NG.COL.HL", KiloGramsOfCo2PerKwh = 1.011511 });
            sources.Add(new Source { SourceName = "Natural Gas", HourlySourceId = "EBA.MISO-ALL.NG.NG.HL", KiloGramsOfCo2PerKwh = 0.4127691 });
            sources.Add(new Source { SourceName = "Nuclear", HourlySourceId = "EBA.MISO-ALL.NG.NUC.HL", KiloGramsOfCo2PerKwh = 0 });
            sources.Add(new Source { SourceName = "Petro", HourlySourceId = "EBA.MISO-ALL.NG.OIL.HL", KiloGramsOfCo2PerKwh = 0.9661517 });
            sources.Add(new Source { SourceName = "Other", HourlySourceId = "EBA.MISO-ALL.NG.OTH.HL", KiloGramsOfCo2PerKwh = 0.30 }); //What to use for other?
            return sources;
        }
    }
}
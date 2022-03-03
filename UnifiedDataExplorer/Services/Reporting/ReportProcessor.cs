using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using EIA.Domain.Model;
using EIA.Services.Clients;
using Microsoft.Extensions.Logging;
using PiModel;
using PiServices;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;

namespace UnifiedDataExplorer.Services.Reporting
{
    public class ReportProcessor
    {
        private readonly PiHttpClient _piClient;
        private readonly EiaClient _eiaClient;
        private readonly IMessageHub _messageHub;
        private readonly ILogger _logger;

        public ReportProcessor(PiHttpClient piClient, EiaClient eiaClient, IMessageHub messageHub, ILogger<ReportProcessor> logger)
        {
            _piClient = piClient;
            _eiaClient = eiaClient;
            _messageHub = messageHub;
            _logger = logger;

            _messageHub.Subscribe<MenuItemEvent>(OnMenuItemEvent);
        }

        private async void OnMenuItemEvent(MenuItemEvent args)
        {
            try
            {
                if (args.Action == MenuItemHeaders.RENDER_BUILDING_REPORT)
                {
                    await RenderBuildingEmissionsReport();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering report");
                throw;
            }
        }

        public async Task RenderBuildingEmissionsReport()
        {
            string macleanLink = "https://pi.facilities.uiowa.edu/piwebapi/elements/F1EmAVYciAZHVU6DzQbJjxTxWwcE7mI49J6RGuHFS_ZKR9xgSVRTTlQyMjU5XFVJLUVORVJHWVxNQUlOIENBTVBVU1xNQUNMRUFOIEhBTEw";
            Asset asset = await _piClient.GetBySelfLink<Asset>(macleanLink);
            await _piClient.LoadAssetValues(asset);

            IEnumerable<HourSummary> summaries = await BuildHourlySources();
        }

        private async Task<IEnumerable<HourSummary>> BuildHourlySources()
        {
            int numberOfHours = 24;
            IEnumerable<HourlySource> sources = BuildSources();

            ICollection<Series> seriesBySource = new List<Series>();
            foreach (HourlySource source in sources)
            {
                Series sourceSeries = await _eiaClient.GetSeriesByIdAsync(source.SourceId);
                seriesBySource.Add(sourceSeries);
            }

            ICollection<HourSummary> summaries = new List<HourSummary>();
            DateTime startingHour = ParseHour(seriesBySource.First().Data.OrderByDescending(x => ParseHour(x.ColumnHeader)).First().ColumnHeader);

            for (int i = 0; i < numberOfHours; i++)
            {
                HourSummary summary = new HourSummary();
                summary.Hour = startingHour;

                for(int j = 0; j < sources.Count(); j++)
                {
                    HourlySource source = sources.ElementAt(j);
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

        private IEnumerable<HourlySource> BuildSources()
        {//https://www.eia.gov/tools/faqs/faq.php?id=74&t=11
            ICollection<HourlySource> sources = new List<HourlySource>();
            sources.Add(new HourlySource { Source = "Wind", SourceId = "EBA.MISO-ALL.NG.WND.HL", PoundsOfCo2PerKwh = 0 });
            sources.Add(new HourlySource { Source = "Solar", SourceId = "EBA.MISO-ALL.NG.SUN.HL", PoundsOfCo2PerKwh = 0 });
            sources.Add(new HourlySource { Source = "Hydro", SourceId = "EBA.MISO-ALL.NG.WAT.HL", PoundsOfCo2PerKwh = 0 });
            sources.Add(new HourlySource { Source = "Coal", SourceId = "EBA.MISO-ALL.NG.COL.HL", PoundsOfCo2PerKwh = 2.23 });
            sources.Add(new HourlySource { Source = "Natural Gas", SourceId = "EBA.MISO-ALL.NG.NG.HL", PoundsOfCo2PerKwh = 0.91 });
            sources.Add(new HourlySource { Source = "Nuclear", SourceId = "EBA.MISO-ALL.NG.NUC.HL", PoundsOfCo2PerKwh = 0 });
            sources.Add(new HourlySource { Source = "Petro", SourceId = "EBA.MISO-ALL.NG.OIL.HL", PoundsOfCo2PerKwh = 2.13 });
            sources.Add(new HourlySource { Source = "Other", SourceId = "EBA.MISO-ALL.NG.OTH.HL", PoundsOfCo2PerKwh = 0.5 }); //What to use for other?
            return sources;
        }

        private class HourSummary
        {
            public DateTime Hour { get; set; }

            public ICollection<HourlySource> Sources { get; } = new List<HourlySource>();
        }

        private class HourlySource
        {
            //Constants
            public string Source { get; set; }
            public string SourceId { get; set; }
            public double PoundsOfCo2PerKwh { get; set; }

            //Changes by hour
            public DateTime Hour { get; set; }
            public double Value { get; set; }

            public HourlySource ProduceHour(DateTime hour, double value)
            {
                return new HourlySource
                {
                    Source = this.Source,
                    SourceId = this.SourceId,
                    PoundsOfCo2PerKwh = this.PoundsOfCo2PerKwh,
                    Hour = hour,
                    Value = value
                };
            }
        }
    }
}

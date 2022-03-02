using System;
using System.Collections.Generic;
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

            string misoWindId = "EBA.MISO-ALL.NG.WND.HL";
            Series series = await _eiaClient.GetSeriesByIdAsync(misoWindId);
        }

        private IEnumerable<HourlySource> BuildSources()
        {
            ICollection<HourlySource> sources = new List<HourlySource>();
            sources.Add(new HourlySource { Source = "Wind", SourceId = "EBA.MISO-ALL.NG.WND.HL", PoundsOfCo2PerKwh = 0 });
            sources.Add(new HourlySource { Source = "Solar", SourceId = "EBA.MISO-ALL.NG.SUN.HL", PoundsOfCo2PerKwh = 0 });
            sources.Add(new HourlySource { Source = "Hydro", SourceId = "EBA.MISO-ALL.NG.WAT.HL", PoundsOfCo2PerKwh = 0 });
            sources.Add(new HourlySource { Source = "Coal", SourceId = "EBA.MISO-ALL.NG.COL.HL", PoundsOfCo2PerKwh = int.MaxValue });
            sources.Add(new HourlySource { Source = "Natural Gas", SourceId = "EBA.MISO-ALL.NG.NG.HL", PoundsOfCo2PerKwh = int.MaxValue });
            sources.Add(new HourlySource { Source = "Nuclear", SourceId = "EBA.MISO-ALL.NG.NUC.HL", PoundsOfCo2PerKwh = int.MaxValue });
            sources.Add(new HourlySource { Source = "Petro", SourceId = "EBA.MISO-ALL.NG.OIL.HL", PoundsOfCo2PerKwh = int.MaxValue });
            sources.Add(new HourlySource { Source = "Other", SourceId = "EBA.MISO-ALL.NG.OTH.HL", PoundsOfCo2PerKwh = int.MaxValue });
            return sources;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}

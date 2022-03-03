using System;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using Microsoft.Extensions.Logging;
using UIowaBuildingsServices;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;

namespace UnifiedDataExplorer.Services.Reporting
{
    public class ReportProcessor
    {
        private readonly ReportingService _reportingService;
        private readonly IMessageHub _messageHub;
        private readonly ILogger _logger;

        public ReportProcessor(ReportingService reportingService, IMessageHub messageHub, ILogger<ReportProcessor> logger)
        {
            _reportingService = reportingService;
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
            await _reportingService.GenerateHourlyEmissionsReport("Put path here");
        }
    }
}

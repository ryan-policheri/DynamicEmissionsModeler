using System;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using DotNetCommon.PersistenceHelpers;
using DotNetCommon.SystemHelpers;
using Microsoft.Extensions.Logging;
using UIowaBuildingsServices;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.Services.DataPersistence;
using UnifiedDataExplorer.Services.Window;

namespace UnifiedDataExplorer.Services.Reporting
{
    public class ReportProcessor
    {
        private readonly ReportingService _reportingService;
        private readonly IMessageHub _messageHub;
        private readonly DataFileProvider _dataFileProvider;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger;

        public ReportProcessor(ReportingService reportingService, IMessageHub messageHub,
            DataFileProvider dataFileProvider, IDialogService dialogService, ILogger<ReportProcessor> logger)
        {
            _reportingService = reportingService;
            _messageHub = messageHub;
            _dataFileProvider = dataFileProvider;
            _dialogService = dialogService;
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

            string reportDirectory = _dataFileProvider.GetReportsDirectory();
            string fileName = "temp" + DateTime.Now.Minute.ToString() + ".xlsx";
            string fullPath = SystemFunctions.CombineDirectoryComponents(reportDirectory, fileName);
            await _reportingService.GenerateHourlyEmissionsReport(fullPath);
            SystemFunctions.OpenFile(fullPath);
        }
    }
}

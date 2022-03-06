﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DotNetCommon.EventAggregation;
using DotNetCommon.PersistenceHelpers;
using DotNetCommon.SystemHelpers;
using PiModel;
using PiServices;
using UIowaBuildingsServices;
using UnifiedDataExplorer.Constants;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.Services.DataPersistence;
using UnifiedDataExplorer.Services.Window;
using UnifiedDataExplorer.ViewModel;
using UIowaBuildingsModel;

namespace UnifiedDataExplorer.Services.Reporting
{
    public class ReportProcessor
    {
        private readonly ReportingService _reportingService;
        private readonly PiHttpClient _piClient;
        private readonly string _rootAssetLink;
        private readonly IMessageHub _messageHub;
        private readonly DataFileProvider _dataFileProvider;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger;

        public ReportProcessor(ReportingService reportingService, PiHttpClient piClient, string rootAssetLink,
            IMessageHub messageHub, DataFileProvider dataFileProvider, IDialogService dialogService, ILogger<ReportProcessor> logger)
        {
            _reportingService = reportingService;
            _piClient = piClient;
            _rootAssetLink = rootAssetLink;
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
            Asset asset = await _piClient.GetByDirectLink<Asset>(_rootAssetLink);
            IEnumerable<Asset> childAssets = await _piClient.GetChildAssets(asset);

            HourlyEmissionsReportParameters model = new HourlyEmissionsReportParameters();
            AppDataFile savedParametersFile = _dataFileProvider.BuildHourlyEmissionsReportDataFile();
            if (savedParametersFile.FileExists) model = await savedParametersFile.ReadAsync<HourlyEmissionsReportParameters>();

            //not loading these from save because want yesterday to be default. Need fancier design to save relative dates like that.
            model.StartDateTime = DateTime.Today.AddDays(-1);
            model.EndDateTime = DateTime.Today;

            HourlyEmissionsReportParametersViewModel vm = new HourlyEmissionsReportParametersViewModel(model, childAssets);
            _dialogService.ShowModalWindow(vm);

            model = vm.ToModel();
            await savedParametersFile.SaveAsync<HourlyEmissionsReportParameters>(model);

            string reportDirectory = _dataFileProvider.GetReportsDirectory();
            string fileName = "temp" + DateTime.Now.Minute.ToString() + ".xlsx";
            string fullPath = SystemFunctions.CombineDirectoryComponents(reportDirectory, fileName);
            await _reportingService.GenerateHourlyEmissionsReport(model, fullPath);
            SystemFunctions.OpenFile(fullPath);
        }
    }
}

﻿using System;
using Microsoft.Extensions.Logging;
using DotNetCommon.EventAggregation;
using UnifiedDataExplorer.Services.WindowDialog;
using UnifiedDataExplorer.Services.DataPersistence;

namespace UnifiedDataExplorer.ViewModel.Base
{
    public class RobustViewModelDependencies
    {
        public RobustViewModelDependencies(IServiceProvider serviceProvider, IMessageHub messageHub,
            ILogger<RobustViewModelDependencies> logger, IDialogService dialogService,
            DataFileProvider provider)
        {
            ServiceProvider = serviceProvider;
            MessageHub = messageHub;
            Logger = logger;
            DialogService = dialogService;
            DataFileProvider = provider;
        }

        public IServiceProvider ServiceProvider { get; }

        public IMessageHub MessageHub { get; }

        public ILogger Logger { get; }

        public IDialogService DialogService { get; }

        public DataFileProvider DataFileProvider { get; }
    }
}
﻿using System;
using System.IO;
using DotNetCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetCommon.Security;
using DotNetCommon.EventAggregation;
using DotNetCommon.Logging.File;
using DotNetCommon.SystemHelpers;
using DotNetCommon.PersistenceHelpers;
using EIA.Services.Clients;
using EmissionsMonitorDataAccess;
using PiServices;
using UIowaBuildingsServices;
using UnifiedDataExplorer.ViewModel;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.MainMenu;
using UnifiedDataExplorer.Services.WindowDialog;
using UnifiedDataExplorer.Services.DataPersistence;
using UnifiedDataExplorer.Services.Reporting;
using UnifiedDataExplorer.ViewModel.DataSources;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess.Http;
using EmissionsMonitorDataAccess.FileSystem;
using EmissionsMonitorServices.DataSourceWrappers;
using UnifiedDataExplorer.ViewModel.DataExploring;
using UnifiedDataExplorer.ViewModel.DataExploring.Explorers;
using UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints;
using UnifiedDataExplorer.ViewModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;
using EmissionsMonitorServices;

namespace UnifiedDataExplorer.Startup
{
    public static class Bootstrapper
    {
        public static Config LoadConfiguration()
        {
            IConfiguration rawConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            Config config = new Config(rawConfig);
            config.AppDataDirectory = SystemFunctions.CombineDirectoryComponents(AppDataFolderOptions.Roaming, "Unified Data Explorer");

            return config;
        }

        public static IServiceProvider BuildServiceProvider(Config config)
        {
            DataFileProvider dataFileProvider = new DataFileProvider(config.AppDataDirectory);

            AppDataFile encryptionKeyFile = dataFileProvider.BuildKeyFile();
            if (!encryptionKeyFile.FileExists)
            {
                EncryptionKey key = CredentialProvider.GenerateIvAndKey();
                encryptionKeyFile.Save(key);
            }

            CredentialProvider credProvider = new CredentialProvider(encryptionKeyFile.FullFilePath);
            dataFileProvider = new DataFileProvider(config.AppDataDirectory, credProvider);

            EncryptedAppDataFile credentialsFile = dataFileProvider.BuildCredentialsFile();
            CredentialConfig credConfig = new CredentialConfig();
            if (credentialsFile.FileExists)
            {
                credConfig = credentialsFile.Read<CredentialConfig>();
            }
            
            ServiceCollection services = new ServiceCollection();

            string logFileName = $"UnifiedDataExplorer_Log_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}.log";
            string logFileDirectory = SystemFunctions.CombineDirectoryComponents(AppDataFolderOptions.Local, "Unified Data Explorer", "Logs");
            FileLoggerConfig fileLoggerConfig = new FileLoggerConfig
            {
                LogDirectory = logFileDirectory,
                LogFileName = logFileName
            };
            FileLoggerProvider fileLoggerProvider = new FileLoggerProvider(fileLoggerConfig);

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(fileLoggerProvider);
            });

            services.AddSingleton<ICredentialProvider>(credProvider);
            services.AddSingleton<DataFileProvider>(dataFileProvider);

            //CLIENTS
            services.AddTransient<EiaClient>();
            services.AddTransient<PiHttpClient>();
            services.AddSingleton<DataSourceServiceFactory>(x =>
                new DataSourceServiceFactory(() => x.GetService<EiaClient>(), 
                    () => x.GetService<PiHttpClient>(),
                    x.GetService<IDataSourceRepository>()));



            //APP SERVICES
            services.AddSingleton<IMessageHub, MessageHub>();
            services.AddSingleton<IDialogService, DialogService>(x => 
                new DialogService(() => x.GetRequiredService<ModalViewModel>(), 
                    () => x.GetRequiredService<SecondaryViewModel>()));

            services.AddTransient<RobustViewModelDependencies>();
            services.AddTransient<RobustViewModelBase>();

            //MAIN
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<ModalViewModel>();
            services.AddTransient<SecondaryViewModel>();

            //DATA SOURCES
            services.AddTransient<DataSourceBaseViewModel>();
            services.AddTransient<EiaDataSourceViewModel>();
            services.AddTransient<PiDataSourceViewModel>();

            //DATA EXPLORATION
            services.AddTransient<DataExplorationManagerViewModel>();
            services.AddTransient<DataExploringHomeViewModel>();
            services.AddTransient<DataSourceManagerViewModel>();
            services.AddTransient<EiaApiExplorerViewModel>();
            services.AddTransient<PiAssetFrameworkExplorerViewModel>();
            services.AddTransient<PiTagExplorerViewModel>();
            services.AddTransient<ExploreSetFileSystemViewModel>();

            //DATASET RENDERING
            services.AddTransient<EiaSeriesExplorePointViewModel>();
            services.AddTransient<PiJsonDisplayExplorePointViewModel>();
            services.AddTransient<PiAssetValuesExplorePointViewModel>();
            services.AddTransient<PiInterpolatedDataExplorePointViewModel>();

            //ENERGY MODELING
            services.AddTransient<ProcessModelMainViewModel>();
            services.AddTransient<ProcessNodesViewModel>();
            services.AddTransient<NodesNavigationViewModel>();
            services.AddTransient<ExchangeNodeViewModel>();
            services.AddTransient<LikeTermAggregatorNodeViewModel>();
            services.AddTransient<StreamSplitterNodeViewModel>();
            services.AddTransient<ProductConversionNodeViewModel>();
            services.AddTransient<UsageAdjusterNodeViewModel>();
            services.AddTransient<ProductSubtractorNodeViewModel>();
            services.AddTransient<MultiSplitterNodeViewModel>();
            services.AddTransient<MultiProductConversionNodeViewModel>();
            services.AddTransient<DataFunctionViewModel>(x => 
                new DataFunctionViewModel(
                    x.GetService<ModelInitializationService>(),
                    x.GetService<DataSourceServiceFactory>().GetDataSourceInfo,
                    x.GetService<RobustViewModelDependencies>()
                ));
            services.AddTransient<EnergyModelFileSystemViewModel>();
            services.AddTransient<ModelInitializationService>();
            services.AddTransient<DynamicCompilerService>();
            services.AddTransient<ExecutionViewModel>();

            //EMISSIONS MONITOR SERVICES
            services.AddTransient<DataSourceClient>(x =>
            {
                var client = new DataSourceClient();
                client.Initialize(config);
                return client;
            });

            services.AddTransient<DataSourceLocal>(x =>
            {
                var client = new DataSourceLocal(x.GetRequiredService<ICredentialProvider>());
                client.Initialize(config);
                return client;
            });

            services.AddTransient<IDataSourceRepository, DataSourceSecureRepository>();

            services.AddTransient<IVirtualFileSystemRepository, VirtualFileSystemClient>((provider =>
            {
                var client = new VirtualFileSystemClient();
                client.Initialize(config);
                return client;
            }));

            services.AddTransient<ModelExecutionClient>((provider =>
            {
                var client = new ModelExecutionClient();
                client.Initialize(config);
                return client;
            }));

            services.AddTransient<ExcelExportService>();
            services.AddSingleton<ReportProcessor>(x => new ReportProcessor(
                x.GetRequiredService<ReportingService>(),
                x.GetRequiredService<ExcelExportService>(),
                x.GetRequiredService<DataSourceServiceFactory>(),
                config.HourlyEmissionsReportRootAssetLink,
                x.GetRequiredService<IMessageHub>(),
                x.GetRequiredService<DataFileProvider>(),
                x.GetRequiredService<IDialogService>(),
                x.GetRequiredService<ILogger<ReportProcessor>>()));
            services.AddSingleton<ReportingService>();

            return services.BuildServiceProvider();
        }
    }
}
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetCommon.Security;
using DotNetCommon.EventAggregation;
using DotNetCommon.Logging.File;
using DotNetCommon.SystemHelpers;
using DotNetCommon.PersistenceHelpers;
using EIA.Services.Clients;
using PiServices;
using UIowaBuildingsServices;
using UnifiedDataExplorer.ViewModel;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.MainMenu;
using UnifiedDataExplorer.Services.Window;
using UnifiedDataExplorer.Services.DataPersistence;
using UnifiedDataExplorer.Services.Reporting;
using UnifiedDataExplorer.ViewModel.DataSources;
using EmissionsMonitorDataAccess.Abstractions;
using EmissiosMonitorDataAccess.Http;

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
            FileLoggerConfig fileLoggerConfig = new FileLoggerConfig(logFileDirectory, logFileName);
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
            services.AddTransient<PiHttpClient>(x => new PiHttpClient(config.PiWebApiBaseAddress,
                credProvider.DecryptValue(credConfig.EncryptedPiUserName),
                credProvider.DecryptValue(credConfig.EncryptedPiPassword),
                config.PiAssestServerName));

            //APP SERVICES
            services.AddSingleton<IMessageHub, MessageHub>();
            services.AddSingleton<IDialogService, DialogService>();

            services.AddTransient<RobustViewModelDependencies>();
            services.AddTransient<RobustViewModelBase>();

            //MAIN
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainMenuViewModel>();

            //DATA SOURCES
            services.AddTransient<EiaDataSourceViewModel>();
            services.AddTransient<PiDataSourceViewModel>();

            //DATA EXPLORATION
            services.AddTransient<DataExplorerViewModel>();
            services.AddTransient<DataExplorerHomeViewModel>();
            services.AddTransient<DataSourceManagerViewModel>();
            services.AddTransient<EiaDatasetFinderViewModel>();
            services.AddTransient<PiDatasetFinderViewModel>();
            services.AddTransient<PiSearchViewModel>();

            //DATASET RENDERING
            services.AddTransient<EiaSeriesViewModel>();
            services.AddTransient<PiJsonDisplayViewModel>();
            services.AddTransient<PiAssetValuesViewModel>();
            services.AddTransient<PiInterpolatedDataViewModel>();

            //EMISSIONS MONITOR SERVICES
            services.AddTransient<IDataSourceRepository, EmissionsMonitorClient>((provider =>
            {
                var client = new EmissionsMonitorClient();
                client.Initialize(config);
                return client;
            }));

            services.AddTransient<ExcelExportService>();
            services.AddSingleton<ReportProcessor>(x => new ReportProcessor(
                x.GetRequiredService<ReportingService>(),
                x.GetRequiredService<ExcelExportService>(),
                x.GetRequiredService<PiHttpClient>(),
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
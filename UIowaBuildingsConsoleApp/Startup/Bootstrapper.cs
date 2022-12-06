using DotNetCommon;
using DotNetCommon.Logging.File;
using DotNetCommon.PersistenceHelpers;
using DotNetCommon.SystemHelpers;
using EIA.Services.Clients;
using EmissionsMonitorDataAccess;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess.Database;
using EmissionsMonitorDataAccess.Database.Repositories;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorServices.DataSourceWrappers;
using EmissionsMonitorServices.Experiments.DailyCarbonTrend;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PiServices;
using UIowaBuildingsModelConsoleApp;
using UIowaBuildingsServices;

namespace UIowaBuildingsConsoleApp.Startup
{
    internal class Bootstrapper
    {
        public static Config LoadConfiguration()
        {
            IConfiguration rawConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            Config config = new Config(rawConfig);
            return config;
        }

        public static IServiceProvider BuildServiceProvider(Config config)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddDbContext<EmissionsMonitorContext>(options => options.UseSqlServer(config.DefaultConnection));
            services.AddTransient<IDataSourceRepository, DataSourceRepository>();
            services.AddTransient<IVirtualFileSystemRepository, VirtualFileSystemRepository>();

            string logFileName = $"UIowaEnergyConsoleApp_Log_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}.log";
            string logFileDirectory = SystemFunctions.CombineDirectoryComponents(AppDataFolderOptions.Local, "UIowa Energy Console App", "Logs");
            FileLoggerConfig fileLoggerConfig = new FileLoggerConfig(logFileDirectory, logFileName);
            FileLoggerProvider fileLoggerProvider = new FileLoggerProvider(fileLoggerConfig);

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(fileLoggerProvider);
            });

            services.AddTransient<EiaClient>();
            services.AddTransient<PiHttpClient>();
            services.AddTransient<DataSourceServiceFactory>(x =>
            {
                var factory = new DataSourceServiceFactory(() => x.GetService<EiaClient>(), () => x.GetService<PiHttpClient>(),
                    x.GetService<IDataSourceRepository>(), false);
                var task = Task.Run(async () => await factory.LoadAllDataSourceServices());
                task.Wait();
                return factory;
            });

            services.AddTransient<ITimeSeriesDataSource, MainDataSourceRepo>();
            services.AddTransient<ModelInitializationService>();
            services.AddTransient<DynamicCompilerService>();
            services.AddTransient<ModelExecutionService>();
            services.AddSingleton<ReportingService>();

            services.AddTransient<DailyCarbonExperimentDriver>();


            return services.BuildServiceProvider();
        }
    }
}

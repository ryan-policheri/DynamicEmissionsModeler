using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetCommon.Security;
using DotNetCommon.EventAggregation;
using DotNetCommon.Logging.File;
using EIA.Services.Clients;
using EIADataViewer.ViewModel;
using EIADataViewer.ViewModel.Base;
using EIADataViewer.ViewModel.MainMenu;

namespace EIADataViewer.Startup
{
    public static class Bootstrapper
    {
        public static IConfiguration LoadConfiguration()
        {
            IConfiguration rawConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            return rawConfig;
        }

        public static IServiceProvider BuildServiceProvider(IConfiguration _config)
        {
            ICredentialProvider credProvider = new CredentialProvider("Personal_IV", "Personal_Key");
            ServiceCollection services = new ServiceCollection();

            string fileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            fileDirectory += "\\EIADataViewer";
            string fileName = $"EIADataViewerLog_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}.log";
            FileLoggerConfig config = new FileLoggerConfig(fileDirectory, fileName);
            FileLoggerProvider fileLoggerProvider = new FileLoggerProvider(config);

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(fileLoggerProvider);
            });

            services.AddTransient<ICredentialProvider, CredentialProvider>(x => new CredentialProvider("Personal_IV", "Personal_Key"));
            services.AddHttpClient("EiaClient", c =>
            {
                string baseAddress = _config["EiaBaseAddress"].TrimEnd('/');
                c.BaseAddress = new Uri(baseAddress);
                c.DefaultRequestHeaders.Add("Subscription-Key", credProvider.DecryptValue(_config["EiaApiKey"]));
            });

            services.AddTransient<EiaClient>(x => new EiaClient(x.GetRequiredService<IHttpClientFactory>().CreateClient("EiaClient")));

            services.AddSingleton<IMessageHub, MessageHub>();

            services.AddTransient<RobustViewModelDependencies>(x => new RobustViewModelDependencies(x.GetRequiredService<IServiceProvider>(), x.GetRequiredService<IMessageHub>(), x.GetRequiredService<ILogger<RobustViewModelDependencies>>()));
            services.AddTransient<RobustViewModelBase>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<DataExplorerViewModel>();
            services.AddTransient<DatasetFinderViewModel>();
            services.AddTransient<SeriesViewModel>();

            return services.BuildServiceProvider();
        }
    }
}

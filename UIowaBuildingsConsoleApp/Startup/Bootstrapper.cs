using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiServices;
using UIowaBuildingsModelConsoleApp;

namespace UIowaBuildingsConsoleApp.Startup
{
    internal class Bootstrapper
    {
        public static IConfiguration LoadConfiguration()
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;

            IConfiguration rawConfig = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", false)
                .Build();

            return rawConfig;
        }

        public static IServiceProvider BuildServiceProvider(Config config)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddTransient<PiHttpClient>(x => new PiHttpClient(config.PiWebApiBaseAddress, config.PiUserName, config.PiPassword));

            return services.BuildServiceProvider();
        }
    }
}

using DotNetCommon.SystemFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIowaBuildingsModel;

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

            services.AddHttpClient("PiClient", c =>
            {
                string baseAddress = config.PiWebApiBase.TrimEnd('/');
                c.BaseAddress = new Uri(baseAddress);
                c.DefaultRequestHeaders.Add("Authorization", config.Base64AuthString);
            });

            services.AddTransient<PiHttpClient>(x => new PiHttpClient(config.PiWebApiBase, config.Base64AuthString));

            return services.BuildServiceProvider();
        }
    }
}

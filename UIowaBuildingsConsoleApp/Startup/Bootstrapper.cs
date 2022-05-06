using DotNetCommon.Logging.File;
using DotNetCommon.PersistenceHelpers;
using DotNetCommon.Security;
using DotNetCommon.SystemHelpers;
using EIA.Services.Clients;
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
            config.AppDataDirectory = SystemFunctions.CombineDirectoryComponents(AppDataFolderOptions.Roaming, "Unified Data Explorer");

            return config;
        }

        public static IServiceProvider BuildServiceProvider(Config config)
        {
            string encryptionFilePath = SystemFunctions.CombineDirectoryComponents(config.AppDataDirectory, "Key.json");
            AppDataFile encryptionKeyFile = new AppDataFile(encryptionFilePath);
            if (!encryptionKeyFile.FileExists)
            {
                EncryptionKey key = CredentialProvider.GenerateIvAndKey();
                encryptionKeyFile.Save(key);
            }

            CredentialProvider credProvider = new CredentialProvider(encryptionKeyFile.FullFilePath);

            string credentialsFilePath = SystemFunctions.CombineDirectoryComponents(config.AppDataDirectory, "Credentials.json");
            EncryptedAppDataFile credentialsFile = new EncryptedAppDataFile(credentialsFilePath, credProvider);
            CredentialConfig credConfig = new CredentialConfig();
            if (credentialsFile.FileExists)
            {
                credConfig = credentialsFile.Read<CredentialConfig>();
            }

            ServiceCollection services = new ServiceCollection();

            string logFileName = $"UIowaBuildingsConsoleApp_Log_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}.log";
            string logFileDirectory = SystemFunctions.CombineDirectoryComponents(AppDataFolderOptions.Local, "UIowa Buildings Console App", "Logs");
            FileLoggerConfig fileLoggerConfig = new FileLoggerConfig(logFileDirectory, logFileName);
            FileLoggerProvider fileLoggerProvider = new FileLoggerProvider(fileLoggerConfig);

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(fileLoggerProvider);
            });

            services.AddSingleton<ICredentialProvider>(credProvider);

            services.AddTransient<EiaClient>(x => new EiaClient(config.EiaWebApiBaseAddress, credProvider.DecryptValue(credConfig.EncryptedEiaWebApiKey)));
            services.AddTransient<PiHttpClient>(x => new PiHttpClient(config.PiWebApiBaseAddress,
                credProvider.DecryptValue(credConfig.EncryptedPiUserName),
                credProvider.DecryptValue(credConfig.EncryptedPiPassword),
                config.PiAssestServerName));

            services.AddSingleton<ReportingService>();

            return services.BuildServiceProvider();
        }
    }
}

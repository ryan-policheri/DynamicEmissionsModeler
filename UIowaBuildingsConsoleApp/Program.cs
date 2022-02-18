using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiServices;
using UIowaBuildingsConsoleApp.Startup;
using UIowaBuildingsModel;

IConfiguration rawConfig = Bootstrapper.LoadConfiguration();
Config config = new Config(rawConfig);
IServiceProvider serviceProvider = Bootstrapper.BuildServiceProvider(config);

PiHttpClient piClient = serviceProvider.GetService<PiHttpClient>();
await piClient.DoSomething();


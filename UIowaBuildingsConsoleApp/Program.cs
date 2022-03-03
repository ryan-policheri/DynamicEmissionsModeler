using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PiModel;
using PiServices;
using UIowaBuildingsConsoleApp.Startup;
using UIowaBuildingsModelConsoleApp;

IConfiguration rawConfig = Bootstrapper.LoadConfiguration();
Config config = new Config(rawConfig);
IServiceProvider serviceProvider = Bootstrapper.BuildServiceProvider(config);

PiHttpClient piClient = serviceProvider.GetService<PiHttpClient>();

await piClient.DoSomething();

Database database = await piClient.DatabaseSearch("ITSNT2259", "UI-Energy");
Console.WriteLine("Database");

IEnumerable<Asset> assets = await piClient.AssetSearchAll("ITSNT2259", "UI-Energy", 3);
Console.WriteLine("Database");
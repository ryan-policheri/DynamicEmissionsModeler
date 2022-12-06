using DotNetCommon.Extensions;
using Microsoft.Extensions.DependencyInjection;
using UIowaBuildingsConsoleApp.Startup;
using EmissionsMonitorModel;
using UIowaBuildingsModelConsoleApp;
using UIowaBuildingsServices;
using UIowaBuildingsConsoleApp.Experiments.Phase1Reporting;
using EmissionsMonitorServices.Experiments.DailyCarbonTrend;
using UIowaBuildingsConsoleApp;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Config config = Bootstrapper.LoadConfiguration();
        IServiceProvider serviceProvider = Bootstrapper.BuildServiceProvider(config);

        string command = args.Length >= 1 ? args[0] : "Phase1Reporting";

        if (command == "Phase1Reporting") await Phase1Reporter.Phase1OldCode(serviceProvider);
        else if (command == "DailyCarbonTrend") await serviceProvider
                .GetRequiredService<DailyCarbonExperimentDriver>()
                .ExecuteExperiement(CommandLineInputHelpers.BindArgumentsToType<DailyCarbonTrendConfig>(args));
        else throw new NotImplementedException($"Command {command} not recognized.");
    }
}
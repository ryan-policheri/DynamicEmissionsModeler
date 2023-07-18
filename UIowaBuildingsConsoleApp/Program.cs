using Microsoft.Extensions.DependencyInjection;
using DotNetCommon.Helpers;
using UIowaBuildingsConsoleApp.Startup;
using UIowaBuildingsModelConsoleApp;
using UIowaBuildingsConsoleApp.Experiments.Phase1Reporting;
using EmissionsMonitorServices.Experiments.DailyCarbonTrend;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using EmissionsMonitorServices.Experiments.IndStudyExp;
using EmissionsMonitorModel.Experiments.IndStudyExp;
using EmissionsMonitorServices.Experiments.NodeInspect;
using EmissionsMonitorModel.Experiments.NodeInspect;

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
                .ExecuteExperiement(CommandLineHelpers.BindArgumentsToType<DailyCarbonExperiment>(args));
        else if (command == "IndStudyExp") await serviceProvider
                .GetRequiredService<IndStudyExpDriver>()
                .ExecuteExperiement(CommandLineHelpers.BindArgumentsToType<IndStudyExperiment>(args));
        else if (command == "NodeInspect") await serviceProvider
                .GetRequiredService<NodeInspectExperimentDriver>()
                .ExecuteExperiement(CommandLineHelpers.BindArgumentsToType<NodeInspectExperiment>(args));
        else throw new NotImplementedException($"Command {command} not recognized.");
    }
}
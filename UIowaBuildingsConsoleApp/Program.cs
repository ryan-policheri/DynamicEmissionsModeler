using Microsoft.Extensions.DependencyInjection;
using UIowaBuildingsConsoleApp.Startup;
using UIowaBuildingsModelConsoleApp;
using UIowaBuildingsConsoleApp.Experiments.Phase1Reporting;
using EmissionsMonitorServices.Experiments.DailyCarbonTrend;
using UIowaBuildingsConsoleApp;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using EmissionsMonitorServices.Experiments.IndStudyExp;
using EmissionsMonitorModel.Experiments.IndStudyExp;

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
                .ExecuteExperiement(CommandLineInputHelpers.BindArgumentsToType<DailyCarbonExperiment>(args));
        else if (command == "IndStudyExp") await serviceProvider
                .GetRequiredService<IndStudyExpDriver>()
                .ExecuteExperiement(CommandLineInputHelpers.BindArgumentsToType<IndStudyExperiment>(args));
        else throw new NotImplementedException($"Command {command} not recognized.");
    }
}
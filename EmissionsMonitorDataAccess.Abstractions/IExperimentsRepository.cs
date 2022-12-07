using EmissionsMonitorModel.Experiments.DailyCarbonTrend;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IExperimentsRepository
    {
        Task SaveDailyCarbonExperimentResultsAsync(DailyCarbonExperiment experiment);
    }

}

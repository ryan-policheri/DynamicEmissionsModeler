using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using EmissionsMonitorModel.Experiments.IndStudyExp;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IExperimentsRepository
    {
        Task SaveDailyCarbonExperimentResultsAsync(DailyCarbonExperiment experiment);
        Task SaveIndStudyExperimentResultsAsync(IndStudyExperiment experiment);
    }

}

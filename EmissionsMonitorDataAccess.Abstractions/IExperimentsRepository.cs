using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using EmissionsMonitorModel.Experiments.IndStudyExp;
using EmissionsMonitorModel.Experiments.NodeInspect;

namespace EmissionsMonitorDataAccess.Abstractions
{
    public interface IExperimentsRepository
    {
        Task SaveDailyCarbonExperimentResultsAsync(DailyCarbonExperiment experiment);
        Task SaveIndStudyExperimentResultsAsync(IndStudyExperiment experiment);
        Task SaveNodeInspectExperimentResults(NodeInspectExperiment experiment);
    }

}

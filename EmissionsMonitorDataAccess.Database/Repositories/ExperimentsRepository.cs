using DotNetCommon.Extensions;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using EmissionsMonitorModel.Experiments.IndStudyExp;
using EmissionsMonitorModel.Experiments.NodeInspect;

namespace EmissionsMonitorDataAccess.Database.Repositories
{
    public class ExperimentsRepository : IExperimentsRepository
    {
        private readonly EmissionsMonitorContext _context;

        public ExperimentsRepository(EmissionsMonitorContext context)
        {
            _context = context;
        }

        public async Task SaveDailyCarbonExperimentResultsAsync(DailyCarbonExperiment experiment)
        {
            _context.Set<DailyCarbonExperiment>().Add(experiment);
            _context.Entry(experiment).Property("NodeIdsString").CurrentValue = experiment?.NodeIds?.ToDelimitedList(",");
            _context.Entry(experiment).Property("ExperimentDate").CurrentValue = DateTimeOffset.Now;
            await _context.SaveChangesAsync();
        }

        public async Task SaveIndStudyExperimentResultsAsync(IndStudyExperiment experiment)
        {
            _context.Set<IndStudyExperiment>().Add(experiment);
            _context.Entry(experiment).Property("ExperimentDate").CurrentValue = DateTimeOffset.Now;
            await _context.SaveChangesAsync();
        }

        public async Task SaveNodeInspectExperimentResults(NodeInspectExperiment experiment)
        {
            _context.Set<NodeInspectExperiment>().Add(experiment);
            _context.Entry(experiment).Property("ExperimentDate").CurrentValue = DateTimeOffset.Now;
            await _context.SaveChangesAsync();
        }
    }
}

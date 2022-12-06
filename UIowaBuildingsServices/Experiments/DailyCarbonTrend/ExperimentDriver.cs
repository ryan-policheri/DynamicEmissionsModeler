using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using Microsoft.Extensions.Logging;
using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorServices.Experiments.DailyCarbonTrend
{
    public class DailyCarbonExperimentDriver
    {
        private readonly IVirtualFileSystemRepository _repo;
        private readonly ModelInitializationService _initService;
        private readonly ModelExecutionService _executionService;
        private readonly ILogger<DailyCarbonExperimentDriver> _logger;

        public DailyCarbonExperimentDriver(IVirtualFileSystemRepository repo, ModelInitializationService initService, ModelExecutionService executionService, ILogger<DailyCarbonExperimentDriver> logger)
        {
            _repo = repo;
            _initService = initService;
            _executionService = executionService;
            _logger = logger;
        }

        public async Task ExecuteExperiement(DailyCarbonTrendConfig config)
        {
            var model = (await _repo.GetModelSaveItemAsync(config.ModelId)).ToProcessModel();
            await _initService.InitializeModel(model);
            var results = await _executionService.ExecuteModelAsync(new ModelExecutionSpec 
            {
                Model = model,
                StartTime = config.StartDate,
                EndTime = config.EndDate,
                DataResolution = DataResolution.Daily
            });
        }
    }
}

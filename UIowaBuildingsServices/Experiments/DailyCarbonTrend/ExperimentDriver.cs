using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess;
using EmissionsMonitorModel.Experiments.DailyCarbonTrend;
using Microsoft.Extensions.Logging;
using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorServices.Experiments.DailyCarbonTrend
{
    public class DailyCarbonExperimentDriver
    {
        private readonly IVirtualFileSystemRepository _fileRepo;
        private readonly IExperimentsRepository _expsRepo;
        private readonly ModelInitializationService _initService;
        private readonly ModelExecutionService _executionService;
        private readonly ILogger<DailyCarbonExperimentDriver> _logger;

        public DailyCarbonExperimentDriver(IVirtualFileSystemRepository fileRepo, IExperimentsRepository expsRepo, ModelInitializationService initService, ModelExecutionService executionService, ILogger<DailyCarbonExperimentDriver> logger)
        {
            _fileRepo = fileRepo;
            _expsRepo = expsRepo;
            _initService = initService;
            _executionService = executionService;
            _logger = logger;
        }

        public async Task ExecuteExperiement(DailyCarbonExperiment experiment)
        {
            var model = (await _fileRepo.GetModelSaveItemAsync(experiment.ModelId)).ToProcessModel();
            await _initService.InitializeModel(model);
            var results = await _executionService.ExecuteModelAsync(new ModelExecutionSpec 
            {
                Model = model,
                StartTime = experiment.StartDate,
                EndTime = experiment.EndDate,
                DataResolution = DataResolution.Daily,
                NodeIds = experiment.NodeIds
            });

            experiment.ExperimentRecords = results.NodeSeries
                .SelectMany(x => x.DataPoints)
                .GroupBy(x => x.Timestamp)
                .Select(g => new DailyCarbonExperimentRecord
                {
                    Date = g.Key,
                    Co2InKilograms = g.Sum(x => x.Values.Costs
                            .First(x => x.UnitType == (new Co2MassFunction()).FunctionUnit && x.UnitForm == (new Co2MassFunction()).FunctionUnitForm)
                            .TotalValue),
                    Co2InMetricTons = g.Sum(x => x.Values.Costs
                            .First(x => x.UnitType == (new Co2MassFunction()).FunctionUnit && x.UnitForm == (new Co2MassFunction()).FunctionUnitForm)
                            .TotalValue) / 1000,
                    FuelCostInDollars = g.Sum(x => x.Values.Costs
                            .First(x => x.UnitType == (new MoneyFunction()).FunctionUnit && x.UnitForm == (new MoneyFunction()).FunctionUnitForm)
                            .TotalValue)
                }).ToList();

            await _expsRepo.SaveDailyCarbonExperimentResultsAsync(experiment);
        }
    }
}

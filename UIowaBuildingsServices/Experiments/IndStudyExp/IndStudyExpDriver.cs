using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess;
using EmissionsMonitorServices.Experiments.DailyCarbonTrend;
using Microsoft.Extensions.Logging;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.Experiments.IndStudyExp;

namespace EmissionsMonitorServices.Experiments.IndStudyExp
{
    public class IndStudyExpDriver
    {
        private readonly IVirtualFileSystemRepository _fileRepo;
        private readonly IExperimentsRepository _expsRepo;
        private readonly ModelInitializationService _initService;
        private readonly ModelExecutionService _executionService;
        private readonly ILogger<DailyCarbonExperimentDriver> _logger;

        public IndStudyExpDriver(IVirtualFileSystemRepository fileRepo, IExperimentsRepository expsRepo, ModelInitializationService initService, ModelExecutionService executionService, ILogger<DailyCarbonExperimentDriver> logger)
        {
            _fileRepo = fileRepo;
            _expsRepo = expsRepo;
            _initService = initService;
            _executionService = executionService;
            _logger = logger;
        }

        public async Task ExecuteExperiement(IndStudyExperiment experiment)
        {
            var model = (await _fileRepo.GetModelSaveItemAsync(experiment.ModelId)).ToProcessModel();
            await _initService.InitializeModel(model);
            var results = await _executionService.ExecuteModelAsync(new ModelExecutionSpec
            {
                Model = model,
                StartTime = experiment.StartDate,
                EndTime = experiment.EndDate,
                DataResolution = experiment.DataResolution,
                NodeIds = new List<int>() { experiment.FinalSteamNodeId, experiment.FinalElectricNodeId, experiment.FinalChilledWaterNodeId }
            });

            var steamNode = results.NodeSeries.First(x => x.NodeId == experiment.FinalSteamNodeId);
            var electricNode = results.NodeSeries.First(x => x.NodeId == experiment.FinalElectricNodeId);
            var chilledWaterNode = results.NodeSeries.First(x => x.NodeId == experiment.FinalChilledWaterNodeId);

            var getCo2Cost = (IEnumerable<DataFunctionResult> costs) => costs.First(x => x.UnitType == (new Co2MassFunction()).FunctionUnit && x.UnitForm == (new Co2MassFunction()).FunctionUnitForm);
            var getMoneyCost = (IEnumerable<DataFunctionResult> costs) => costs.First(x => x.UnitType == (new MoneyFunction()).FunctionUnit && x.UnitForm == (new MoneyFunction()).FunctionUnitForm);

            List<IndStudyExperimentRecord> records = new List<IndStudyExperimentRecord>();
            foreach (DateTimeOffset timeStamp in results.NodeSeries.First().NodeOutputPoints.Select(x => x.Timestamp)) 
            {
                var steamPoint = steamNode.NodeOutputPoints.First(x => x.Timestamp == timeStamp).Values;
                var electricPoint = electricNode.NodeOutputPoints.First(x => x.Timestamp == timeStamp).Values;
                var chilledWaterPoint = chilledWaterNode.NodeOutputPoints.First(x => x.Timestamp == timeStamp).Values;

                var record = new IndStudyExperimentRecord
                {
                    Timestamp = timeStamp,
                    SteamTotalInMMBTU = steamPoint.Product.TotalValue,
                    SteamEmissionsInMetricTons = getCo2Cost(steamPoint.Costs).TotalValue / 1000,
                    SteamCostInDollars = getMoneyCost(steamPoint.Costs).TotalValue,
                    SteamEmissionsFactorInKilogramsCo2PerMMBTU = steamPoint.CalculateProductCostFactor(getCo2Cost(steamPoint.Costs)),
                    SteamCostDollarsPerMMBTU = steamPoint.CalculateProductCostFactor(getMoneyCost(steamPoint.Costs)),
                    ElectricTotalInKwh = steamPoint.Product.TotalValue / 3.6,
                    ElectricEmissionsInMetricTons = getCo2Cost(electricPoint.Costs).TotalValue / 1000,
                    ElectricCostInDollars = getMoneyCost(electricPoint.Costs).TotalValue,
                    ElectricEmissionsFactorInKilogramsCo2PerKwh = getCo2Cost(electricPoint.Costs).TotalValue / (electricPoint.Product.TotalValue / 3.6),
                    ElectricCostPerKwh = getMoneyCost(electricPoint.Costs).TotalValue / (electricPoint.Product.TotalValue / 3.6),
                    ChilledWaterTotalInGallons = chilledWaterPoint.Product.TotalValue,
                    ChilledWaterEmissionsInMetricTons = getCo2Cost(chilledWaterPoint.Costs).TotalValue / 1000,
                    ChilledWaterCostInDollars = getMoneyCost(chilledWaterPoint.Costs).TotalValue,
                    ChilledWaterEmissionsFactorInKilogramsCo2PerGallon = chilledWaterPoint.CalculateProductCostFactor(getCo2Cost(chilledWaterPoint.Costs)),
                    ChilledWaterCostPerGallon = chilledWaterPoint.CalculateProductCostFactor(getMoneyCost(chilledWaterPoint.Costs)),
                };

                record.TotalEmissionsInMetricTons = record.SteamEmissionsInMetricTons + record.ElectricEmissionsInMetricTons + record.ChilledWaterEmissionsInMetricTons;
                record.TotalCostInDollars = record.SteamCostInDollars + record.ElectricCostInDollars + record.ChilledWaterCostInDollars;

                records.Add(record);
            }

            experiment.ExperimentRecords = records;
            await _expsRepo.SaveIndStudyExperimentResultsAsync(experiment);
        }

    }
}

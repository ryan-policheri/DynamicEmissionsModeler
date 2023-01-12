using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess;
using Microsoft.Extensions.Logging;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.Experiments.NodeInspect;

namespace EmissionsMonitorServices.Experiments.NodeInspect
{
    public class NodeInspectExperimentDriver
    {
        private readonly IVirtualFileSystemRepository _fileRepo;
        private readonly IExperimentsRepository _expsRepo;
        private readonly ModelInitializationService _initService;
        private readonly ModelExecutionService _executionService;
        private readonly ILogger<NodeInspectExperimentDriver> _logger;

        public NodeInspectExperimentDriver(IVirtualFileSystemRepository fileRepo, IExperimentsRepository expsRepo, ModelInitializationService initService, ModelExecutionService executionService, ILogger<NodeInspectExperimentDriver> logger)
        {
            _fileRepo = fileRepo;
            _expsRepo = expsRepo;
            _initService = initService;
            _executionService = executionService;
            _logger = logger;
        }

        public async Task ExecuteExperiement(NodeInspectExperiment experiment)
        {
            var model = (await _fileRepo.GetModelSaveItemAsync(experiment.ModelId)).ToProcessModel();
            await _initService.InitializeModel(model);
            var results = await _executionService.ExecuteModelAsync(new ModelExecutionSpec
            {
                Model = model,
                StartTime = experiment.StartDate,
                EndTime = experiment.EndDate,
                DataResolution = experiment.DataResolution,
                NodeIds = new List<int>() { experiment.NodeId },
                OverflowHandleStrategy = experiment.OverflowStrategy
            });

            if (results.Errors != null)
            {
                foreach (var error in results.Errors)
                {
                    Console.WriteLine($"Overflow error at {error.TimeStamp} in node {error.NodeName}");
                }
            }

            var node = results.NodeSeries.First(x => x.NodeId == experiment.NodeId);

            var getCo2Cost = (IEnumerable<DataFunctionResult> costs) => costs.First(x => x.UnitType == (new Co2MassFunction()).FunctionUnit && x.UnitForm == (new Co2MassFunction()).FunctionUnitForm);
            var getMoneyCost = (IEnumerable<DataFunctionResult> costs) => costs.First(x => x.UnitType == (new MoneyFunction()).FunctionUnit && x.UnitForm == (new MoneyFunction()).FunctionUnitForm);

            List<NodeInspectExperimentRecord> records = new List<NodeInspectExperimentRecord>();
            foreach (DateTimeOffset timeStamp in results.NodeSeries.First().NodeOutputPoints.Select(x => x.Timestamp))
            {
                var nodePoint = node.NodeOutputPoints.First(x => x.Timestamp == timeStamp).Values;

                var record = new NodeInspectExperimentRecord
                {
                    Timestamp = timeStamp,
                    ProductTotalInDefaultUnit = nodePoint.Product.TotalValue,
                    ProductDefaultUnit = nodePoint.Product.DefaultValueUnit,
                    CO2EmissionsInKilograms = getCo2Cost(nodePoint.Costs).TotalValue,
                    FuelCostInDollars = getMoneyCost(nodePoint.Costs).TotalValue,
                    CO2EmissionsPerDefaultProductUnit = nodePoint.Product.TotalValue > 0 ? nodePoint.CalculateProductCostFactor(getCo2Cost(nodePoint.Costs)) : 0,
                    FuelCostPerDefaultProductUnit = nodePoint.Product.TotalValue > 0 ? nodePoint.CalculateProductCostFactor(getMoneyCost(nodePoint.Costs)) : 0
                };

                records.Add(record);
            }

            experiment.ExperimentRecords = records;
            await _expsRepo.SaveNodeInspectExperimentResults(experiment);
        }
    }
}

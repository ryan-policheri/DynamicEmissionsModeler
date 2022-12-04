using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class UsageAdjusterNode : ProcessNode, ISinglePredecessor
    {
        public UsageAdjusterNode()
        {
            PrecedingNodeId = -1;
            PrecedingNode = null;
        }

        public int PrecedingNodeId { get; set; }

        [JsonIgnore]
        public ProcessNode PrecedingNode { get; set; }

        public DataFunction ProductUsageFunction { get; set; }


        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>() { ProductUsageFunction };
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults preceedingStream = PrecedingNode.RenderProductAndCosts(dataPoints);
            if (preceedingStream.Product.UnitType != ProductUsageFunction.FunctionUnit
                || preceedingStream.Product.UnitForm != ProductUsageFunction.FunctionUnitForm
                || preceedingStream.Product.DefaultValueUnit != ProductUsageFunction.FunctionDefaultReturnUnit)
            {
                throw new InvalidOperationException("The preceeding node and the actual usage function must have the same type of product");
            }

            var input = dataPoints.Where(x => ProductUsageFunction.FunctionFactors.Any(y => y.FactorUri.EquivelentSeriesAndConfig(x.Series.SeriesUri)));
            DataFunctionResult actualProductUsage = ProductUsageFunction.ExecuteFunction(input);
            if (preceedingStream.Product.TotalValue < actualProductUsage.TotalValue) { throw new InvalidOperationException("Actual usage value cannot be more than total value"); }

            var costs = preceedingStream.CalculateCostOfRawProductAmount(actualProductUsage.TotalValue);

            ICollection<DataFunctionResult> costsForActualUsage = new List<DataFunctionResult>();
            foreach (var cost in preceedingStream.Costs)
            {
                var result = cost.Duplicate();
                result.TotalValue = costs.First(x => x.CostFunctionName == cost.FunctionName).Value;
                costsForActualUsage.Add(result);
            }

            ProductCostResults actualUsageResults = new ProductCostResults
            {
                Product = actualProductUsage,
                Costs = costsForActualUsage
            };

            return actualUsageResults;
        }
    }
}

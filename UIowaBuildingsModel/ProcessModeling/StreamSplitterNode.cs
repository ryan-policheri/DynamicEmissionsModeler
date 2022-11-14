using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class StreamSplitterNode : ProcessNode
    {
        public StreamSplitterNode()
        {
            PrecedingNodeId = -1;
            PrecedingNode = null;
            SplitResultNode = new StreamSplitResultNode { Id = this.SplitResultNodeId, OwningSplitterId = this.SplitResultNodeId, OwningSplitter = this, Name = this.Name + " (Split)" };
            RemainderResultNode = new StreamSplitRemainderNode { Id = this.RemainderResultNodeId, OwningSplitterId = this.SplitResultNodeId, OwningSplitter = this, Name = this.Name + " (Remainder)" };
        }

        public int PrecedingNodeId { get; set; }

        [JsonIgnore]
        public ProcessNode PrecedingNode { get; set; }

        public int SplitResultNodeId { get; set; }

        [JsonIgnore]
        public StreamSplitResultNode SplitResultNode { get; set; }

        public int RemainderResultNodeId { get; set; }

        [JsonIgnore]
        public StreamSplitRemainderNode RemainderResultNode { get; set; }

        public DataFunction SplitFunction { get; set; }

        public ProductCostResults RenderSplitFunctionProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults preceedingStream = RenderProductAndCosts(dataPoints);
            if(preceedingStream.Product.UnitType != SplitFunction.FunctionUnit || preceedingStream.Product.UnitForm != SplitFunction.FunctionUnitForm)
            {
                throw new InvalidOperationException("The proceeding node and the split function must have the same type of product");
            }

            var inputPoints = dataPoints.Where(x => SplitFunction.FunctionFactors.Any(y => y.FactorUri.Uri == x.Series.SeriesUri.Uri));
            DataFunctionResult splitProduct = SplitFunction.ExecuteFunction(inputPoints);
            if (preceedingStream.Product.TotalValue < splitProduct.TotalValue) { throw new InvalidOperationException("Split value cannot be more than total value"); }
           
            var costs = preceedingStream.CalculateCostOfRawProductAmount(splitProduct.TotalValue);
            ICollection<DataFunctionResult> splitCosts = new List<DataFunctionResult>();
            foreach (var cost in preceedingStream.Costs)
            {
                var result = cost.Duplicate();
                result.FunctionName += $" (Split by {this.SplitFunction.FunctionName})";
                result.TotalValue = costs.First(x => x.CostFunctionName == cost.FunctionName).Value;
                splitCosts.Add(result);
            }

            ProductCostResults splitProductCostResults = new ProductCostResults
            {
                Product = splitProduct,
                Costs = splitCosts
            };

            return splitProductCostResults;
        }

        public ProductCostResults RenderRemainderProductAndCost(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults preceedingStream = RenderProductAndCosts(dataPoints);
            if (preceedingStream.Product.UnitType != SplitFunction.FunctionUnit 
                || preceedingStream.Product.UnitForm != SplitFunction.FunctionUnitForm
                || preceedingStream.Product.DefaultValueUnit != SplitFunction.FunctionDefaultReturnUnit)
            {
                throw new InvalidOperationException("The proceeding node and the split function must have the same type of product");
            }

            var inputPoints = dataPoints.Where(x => SplitFunction.FunctionFactors.Any(y => y.FactorUri.Uri == x.Series.SeriesUri.Uri));
            DataFunctionResult splitProduct = SplitFunction.ExecuteFunction(inputPoints);
            if(preceedingStream.Product.TotalValue < splitProduct.TotalValue) { throw new InvalidOperationException("Split value cannot be more than total value"); } 
           
            double remaininingValue = preceedingStream.Product.TotalValue - splitProduct.TotalValue;
            DataFunctionResult remainingProduct = preceedingStream.Product.Duplicate();
            remainingProduct.FunctionName += $" (Remainder of {this.SplitFunction.FunctionName})";
            remainingProduct.TotalValue = remaininingValue;

            var costs = preceedingStream.CalculateCostOfRawProductAmount(remaininingValue);
            ICollection<DataFunctionResult> remainderCosts = new List<DataFunctionResult>();
            foreach (var cost in preceedingStream.Costs)
            {
                var remainingCost = cost.Duplicate();
                remainingCost.FunctionName += $" (Remainder of {this.SplitFunction.FunctionName})";
                remainingCost.TotalValue = costs.First(x => x.CostFunctionName == cost.FunctionName).Value;
                remainderCosts.Add(remainingCost);
            }

            ProductCostResults remainingProductCostResults = new ProductCostResults
            {
                Product = remainingProduct,
                Costs = remainderCosts
            };

            return remainingProductCostResults;
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            return PrecedingNode.RenderProductAndCosts(dataPoints);
        }

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>() { SplitFunction };
        }
    }
}

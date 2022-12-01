using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class MultiSplitterNode : ProcessNode
    {
        public MultiSplitterNode()
        {
            PrecedingNodeId = -1;
            PrecedingNode = null;

            SplitFunctions = new List<DataFunction>();

            SplitResultNodeIds = new List<int>();
            SplitResultNodes = new List<MultiSplitResultNode>();

            RemainderResultNode = new MultiSplitRemainderNode { OwningSplitter = this };
        }

        public new int Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;
                RemainderResultNode.OwningSplitterId = this.Id;
            }
        }

        public new string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                RemainderResultNode.Name = this.Name + " (Remainder)";
            }
        }


        public int PrecedingNodeId { get; set; }

        [JsonIgnore]
        public ProcessNode PrecedingNode { get; set; }

        public ICollection<DataFunction> SplitFunctions { get; set; }

        public ICollection<int> SplitResultNodeIds { get; set; }

        [JsonIgnore]
        public ICollection<MultiSplitResultNode> SplitResultNodes { get; set; }


        private int _remainderResultNodeId;
        public int RemainderResultNodeId
        {
            get { return _remainderResultNodeId; }
            set
            {
                _remainderResultNodeId = value;
                RemainderResultNode.Id = this.RemainderResultNodeId;
            }
        }

        [JsonIgnore]
        public MultiSplitRemainderNode RemainderResultNode { get; set; }

        //TODO: This is smelly. It's done this way because the process model listens to the event so that it can assign an id to the dynamically added node.
        public delegate void ChildNodeDynamicallyAdded(MultiSplitterNode sender, MultiSplitResultNode addedNode);
        public event ChildNodeDynamicallyAdded OnChildNodeDynamicallyAdded;
        public void AddSplitFunction(DataFunction splitFunction)
        {
            SplitFunctions.Add(splitFunction);
            var splitResultNode = new MultiSplitResultNode { Name = splitFunction.FunctionName + " (From Multi Split)", OwningSplitter = this, OwningSplitterId = this.Id, SplitFunctionName = splitFunction.FunctionName };
            SplitResultNodes.Add(splitResultNode);
            if (OnChildNodeDynamicallyAdded != null) OnChildNodeDynamicallyAdded(this, splitResultNode);
        }

        public ProductCostResults RenderSplitFunctionProductAndCosts(ICollection<DataPoint> dataPoints, MultiSplitResultNode resultNode)
        {
            ProductCostResults preceedingStream = RenderProductAndCosts(dataPoints);
            var splitFunction = this.SplitFunctions.First(x => x.FunctionName == resultNode.SplitFunctionName);

            if (preceedingStream.Product.UnitType != splitFunction.FunctionUnit || preceedingStream.Product.UnitForm != splitFunction.FunctionUnitForm)
            {
                throw new InvalidOperationException("The proceeding node and the split function must have the same type of product");
            }

            var inputPoints = dataPoints.Where(x => splitFunction.FunctionFactors.Any(y => y.FactorUri.Uri == x.Series.SeriesUri.Uri));
            DataFunctionResult splitProduct = splitFunction.ExecuteFunction(inputPoints);
            if (preceedingStream.Product.TotalValue < splitProduct.TotalValue) { throw new InvalidOperationException("Split value cannot be more than total value"); }

            var costs = preceedingStream.CalculateCostOfRawProductAmount(splitProduct.TotalValue);
            ICollection<DataFunctionResult> splitCosts = new List<DataFunctionResult>();
            foreach (var cost in preceedingStream.Costs)
            {
                var result = cost.Duplicate();
                result.FunctionName += $" (Split by {splitFunction.FunctionName})";
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
            IEnumerable<ProductCostResults> allSplits = this.SplitResultNodes.Select(x => x.RenderProductAndCosts(dataPoints));

            if (preceedingStream.Product.TotalValue < allSplits.Sum(x => x.Product.TotalValue)) { throw new InvalidOperationException("Split value cannot be more than total value"); }

            double remaininingValue = preceedingStream.Product.TotalValue - allSplits.Sum(x => x.Product.TotalValue);
            DataFunctionResult remainingProduct = preceedingStream.Product.Duplicate();
            remainingProduct.FunctionName += $" (Remainder of {this.Name})";
            remainingProduct.TotalValue = remaininingValue;

            var costs = preceedingStream.CalculateCostOfRawProductAmount(remaininingValue);
            ICollection<DataFunctionResult> remainderCosts = new List<DataFunctionResult>();
            foreach (var cost in preceedingStream.Costs)
            {
                var remainingCost = cost.Duplicate();
                remainingCost.FunctionName += $" (Remainder of {this.Name})";
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

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return SplitFunctions.ToList();
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            return PrecedingNode.RenderProductAndCosts(dataPoints);
        }
    }
}

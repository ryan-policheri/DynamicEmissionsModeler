﻿using EmissionsMonitorModel.Exceptions;
using EmissionsMonitorModel.TimeSeries;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class MultiSplitterNode : ProcessNode, ISinglePredecessor, ICreateAncillaryNodes
    {
        public MultiSplitterNode()
        {
            PrecedingNodeId = -1;
            PrecedingNode = null;

            SplitFunctions = new List<DataFunction>();
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
        //It'd be better to just use guids instead of integer ids that way we can just assign the id here
        public delegate void ChildNodeDynamicallyAdded(MultiSplitterNode sender, MultiSplitResultNode addedNode);
        public event ChildNodeDynamicallyAdded OnChildNodeDynamicallyAdded;

        public void AddSplitFunction(DataFunction splitFunction)
        {
            SplitFunctions.Add(splitFunction);
            var splitResultNode = new MultiSplitResultNode
            {
                OwningSplitter = this,
                OwningSplitterId = this.Id,
                Name = splitFunction.FunctionName + " (From Multi Split)",
                SplitFunctionName = splitFunction.FunctionName
            };
            SplitResultNodes.Add(splitResultNode);
            if (OnChildNodeDynamicallyAdded != null) OnChildNodeDynamicallyAdded(this, splitResultNode);
        }

        public void UpdateSplitFunction(DataFunction oldSplitFunction, DataFunction newSplitFunction)
        {
            MultiSplitResultNode relatedResultNode = this.SplitResultNodes.First(x => x.SplitFunctionName == oldSplitFunction.FunctionName);
            this.SplitFunctions.Remove(oldSplitFunction);
            this.SplitFunctions.Add(newSplitFunction);
            relatedResultNode.Name = newSplitFunction.FunctionName + " (From Multi Split)";
            relatedResultNode.SplitFunctionName = newSplitFunction.FunctionName;
        }

        public void RemoveSplitFunction(DataFunction function)
        {
            MultiSplitResultNode relatedResultNode = this.SplitResultNodes.First(x => x.SplitFunctionName == function.FunctionName);
            this.SplitFunctions.Remove(function);
            this.SplitResultNodes.Remove(relatedResultNode);
        }

        public ProductCostResults RenderSplitFunctionProductAndCosts(ICollection<DataPoint> dataPoints, MultiSplitResultNode resultNode)
        {
            ProductCostResults preceedingStream = RenderProductAndCosts(dataPoints);
            var splitFunction = this.SplitFunctions.First(x => x.FunctionName == resultNode.SplitFunctionName);

            if (preceedingStream.Product.UnitType != splitFunction.FunctionUnit || preceedingStream.Product.UnitForm != splitFunction.FunctionUnitForm)
            {
                throw new InvalidOperationException("The proceeding node and the split function must have the same type of product");
            }

            var inputPoints = dataPoints.Where(x => splitFunction.FunctionFactors.Any(y => y.FactorUri.EquivelentSeriesAndConfig(x.Series.SeriesUri)));
            DataFunctionResult splitProduct = splitFunction.ExecuteFunction(inputPoints);
            if (preceedingStream.Product.TotalValue < splitProduct.TotalValue) 
            {
                throw new NodeOverflowException(new NodeOverflowError { NodeId = this.Id, NodeName = this.Name,
                    NodeInputs = dataPoints, TimeStamp = dataPoints.First().Timestamp }, 
                    "Split value cannot be more than total value", null); 
            }

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
            ICollection<ProductCostResults> allSplits = this.SplitResultNodes.Select(x => x.RenderProductAndCosts(dataPoints)).ToList();
            var sum = allSplits.Sum(x => x.Product.TotalValue);
            var prec = preceedingStream.Product.TotalValue;

            if (preceedingStream.Product.TotalValue < allSplits.Sum(x => x.Product.TotalValue)) 
            {
                throw new NodeOverflowException(new NodeOverflowError
                {
                    NodeId = this.Id,
                    NodeName = this.Name,
                    NodeInputs = dataPoints,
                    TimeStamp = dataPoints.First().Timestamp
                }, "Split value cannot be more than total value", null);
            }

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

        public ICollection<ProcessNode> GetAncillaryNodes()
        {
            List<ProcessNode> ancillaryNodes = new List<ProcessNode>();
            foreach (var child in this.SplitResultNodes) ancillaryNodes.Add(child);
            ancillaryNodes.Add(this.RemainderResultNode);
            return ancillaryNodes;
        }
    }
}

﻿using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class LikeTermsAggregatorNode : ProcessNode, IMultiplePredecessor
    {
        public LikeTermsAggregatorNode()
        {
            PrecedingNodeIds = new List<int>();
            PrecedingNodes = new List<ProcessNode>();
        }

        public List<int> PrecedingNodeIds { get; set; }

        [JsonIgnore]
        public List<ProcessNode> PrecedingNodes { get; set; }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults results = new ProductCostResults();

            foreach (ProcessNode precNode in PrecedingNodes)
            {
                ProductCostResults precedingResult = precNode.RenderProductAndCosts(dataPoints);
                if (results.Product == null) results.Product = precedingResult.Product;
                else
                {
                    if (!results.Product.IsLikeResult(precedingResult.Product)) throw new InvalidOperationException("Products of preceding nodes must be like units");
                    results.Product = DataFunctionResult.CombineResults(results.Product, precedingResult.Product);
                }

                if (results.Costs == null) results.Costs = precedingResult.Costs;
                else
                {
                    foreach (DataFunctionResult costResult in precedingResult.Costs)
                    {
                        //TODO: Does not work if multiple costs exist of the same type on the same preceding node
                        var existingTerm = results.Costs.FirstOrDefault(x => x.IsLikeResult(costResult));
                        if (existingTerm == null) results.Costs.Add(costResult);
                        else
                        {
                            results.Costs.Remove(existingTerm);
                            var combinedResult = DataFunctionResult.CombineResults(existingTerm, costResult);
                            results.Costs.Add(combinedResult);
                        }
                    }
                }
            }

            return results;
        }


        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>();
        }
    }
}

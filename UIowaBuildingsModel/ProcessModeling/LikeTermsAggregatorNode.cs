using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class LikeTermsAggregatorNode : ProcessNode
    {
        public List<ExchangeNode> PrecedingNodes { get; set; }

        public ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults results = new ProductCostResults();

            foreach (ExchangeNode precNode in PrecedingNodes)
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
    }
}

using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class MultiProductConversionNode : ProcessNode, IMultiplePredecessor
    {//Converts n products to another product (I.E. Electric and Steam to Chilled Water)

        public MultiProductConversionNode()
        {
            PrecedingNodeIds = new List<int>();
            PrecedingNodes = new List<ProcessNode>();
        }

        public List<int> PrecedingNodeIds { get; set; }

        [JsonIgnore]
        public List<ProcessNode> PrecedingNodes { get; set; }

        public DataFunction NewProductFunction { get; set; }

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>() { NewProductFunction };
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults results = new ProductCostResults();

            //Add up the previous costs
            foreach (ProcessNode precNode in PrecedingNodes)
            {
                ProductCostResults precedingResult = precNode.RenderProductAndCosts(dataPoints);

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

            //Associate the new product with the previous costs
            var input = dataPoints.Where(x => NewProductFunction.FunctionFactors.Any(y => y.FactorUri.Uri == x.Series.SeriesUri.Uri));
            DataFunctionResult newProduct = NewProductFunction.ExecuteFunction(input);
            results.Product = newProduct;

            return results;
        }
    }
}

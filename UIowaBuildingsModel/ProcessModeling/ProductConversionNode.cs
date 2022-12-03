using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProductConversionNode : ProcessNode, ISinglePredecessor
    {//Converts product A to a product B

        public ProductConversionNode()
        {
            PrecedingNodeId = -1;
            PrecedingNode = null;
        }

        public int PrecedingNodeId { get; set; }

        [JsonIgnore]
        public ProcessNode PrecedingNode { get; set; }

        public DataFunction NewProductFunction { get; set; }

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>() { NewProductFunction };
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults results = PrecedingNode.RenderProductAndCosts(dataPoints);
            var input = dataPoints.Where(x => NewProductFunction.FunctionFactors.Any(y => y.FactorUri.Uri == x.Series.SeriesUri.Uri));
            DataFunctionResult newProduct = NewProductFunction.ExecuteFunction(input);
            results.Product = newProduct;
            return results;
        }
    }
}

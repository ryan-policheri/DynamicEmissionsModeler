using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ExchangeNode : ProcessNode
    {
        public ExchangeNode()
        {
        }

        public ICollection<DataFunction> Costs { get; set; }
        public DataFunction Product { get; set; }

        public ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ICollection<DataFunctionResult> costs = new List<DataFunctionResult>();

            foreach (DataFunction cost in Costs)
            {
                var input = dataPoints.Where(x => cost.FunctionFactors.Any(y => y.FactorUri == x.SeriesName));
                DataFunctionResult result = cost.ExecuteFunction(input);
                costs.Add(result);
            }

            var input2 = dataPoints.Where(x => Product.FunctionFactors.Any(y => y.FactorUri == x.SeriesName));
            DataFunctionResult product = Product.ExecuteFunction(input2);

            return new ProductCostResults
            {
                Costs = costs,
                Product = product
            };
        }
    }


}

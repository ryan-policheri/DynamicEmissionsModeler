using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ExchangeNode : ProcessNode
    {
        public ExchangeNode()
        {
            Costs = new List<DataFunction>();
        }

        public ICollection<DataFunction> Costs { get; set; }
        public DataFunction Product { get; set; }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ICollection<DataFunctionResult> costs = new List<DataFunctionResult>();

            foreach (DataFunction cost in Costs)
            {
                var input = dataPoints.Where(x => cost.FunctionFactors.Any(y => y.FactorUri.EquivelentSeriesAndConfig(x.Series.SeriesUri)));
                DataFunctionResult result = cost.ExecuteFunction(input);
                costs.Add(result);
            }

            var input2 = dataPoints.Where(x => Product.FunctionFactors.Any(y => y.FactorUri.EquivelentSeriesAndConfig(x.Series.SeriesUri)));
            DataFunctionResult product = Product.ExecuteFunction(input2);

            return new ProductCostResults
            {
                Costs = costs,
                Product = product
            };
        }

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            List<DataFunction> functions = new List<DataFunction>();
            functions.Add(Product);
            foreach(DataFunction cost in Costs) functions.Add(cost);
            return functions;
        }
    }


}

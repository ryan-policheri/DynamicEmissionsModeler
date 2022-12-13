namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProductCostResults
    {
        public ICollection<DataFunctionResult> Costs { get; set; }

        public DataFunctionResult Product { get; set; }

        public double CalculateProductCostFactor(DataFunctionResult cost) 
            => this.Costs.Contains(cost) ? cost.TotalValue / this.Product.TotalValue : throw new ArgumentOutOfRangeException("This is not my cost!");

        public ICollection<Cost> CalculateCostOfAbstractProductAmount(object productAmount)
        {
            double desiredProductAmount = Product.ValueRenderFunction(productAmount);
            return CalculateCostOfRawProductAmount(desiredProductAmount);
        }

        public ICollection<Cost> CalculateCostOfRawProductAmount(double desiredProductAmount)
        {
            ICollection<Cost> costs = new List<Cost>();
            foreach (DataFunctionResult cost in Costs)
            {
                double coefficient = cost.TotalValue / Product.TotalValue;
                Cost partialCost = new Cost
                {
                    CostFunctionName = cost.FunctionName,
                    Name = cost.UnitForm + " " + cost.UnitType,
                    Value = coefficient * desiredProductAmount
                };

                costs.Add(partialCost);
            }
            return costs;
        }
    }
}

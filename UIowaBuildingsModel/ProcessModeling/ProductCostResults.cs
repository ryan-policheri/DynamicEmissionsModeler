namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProductCostResults
    {
        public ICollection<DataFunctionResult> Costs { get; set; }

        public DataFunctionResult Product { get; set; }

        public ICollection<Cost> CalculateCostOfProductAmount(object productAmount)
        {
            double desiredProductAmount = Product.ValueRenderFunction(productAmount);

            ICollection<Cost> costs = new List<Cost>();
            foreach (DataFunctionResult cost in Costs)
            {
                double coefficient = cost.TotalValue / Product.TotalValue;
                Cost partialCost = new Cost
                {
                    Name = cost.UnitForm + " " + cost.Unit,
                    Value = coefficient * desiredProductAmount
                };
                
                costs.Add(partialCost);
            }
            return costs;
        }
    }
}

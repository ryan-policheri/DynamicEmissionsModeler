using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.Units;
using UnitsNet;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ExchangeNode : ProcessNode
    {
        public ExchangeNode()
        {
        }

        public ICollection<ExchangeCostBase> Costs { get; set; }
        public ExchangeCostBase Product { get; set; }

        public ProductCostConverter RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ICollection<Cost> acutalCosts = new List<Cost>();

            foreach (ExchangeCostBase cost in Costs)
            {
                Cost acutalCost = new Cost();
                acutalCost.Name = cost.CostName;
                var input = dataPoints.Where(x => cost.CostFactors.Any(y => y.FactorUri == x.SeriesName));
                acutalCost.Value = cost.FactorsToCostFunction.Execute(input.First());
                acutalCosts.Add(acutalCost);
            }

            Product actualProduct = new Product();
            var input2 = dataPoints.Where(x => Product.CostFactors.Any(y => y.FactorUri == x.SeriesName));
            actualProduct.Value = Product.FactorsToCostFunction.Execute(input2.First());

            return new ProductCostConverter
            {
                Costs = acutalCosts,
                Product = actualProduct
            };
        }
    }


}

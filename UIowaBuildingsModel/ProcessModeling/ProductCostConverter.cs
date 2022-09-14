using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.Units;
using UnitsNet;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProductCostConverter
    {
        public ICollection<Cost> Costs { get; set; }

        public Product Product { get; set; }

        public ICollection<Cost> CalculateCostOfProductAmount(object productAmount)
        {
            double desiredMmbtu = 0;
            if (productAmount.GetType() == typeof(Energy))
            {
                desiredMmbtu = ((Energy)productAmount).MegabritishThermalUnits;
            }

            double totalMMbtu = 0;
            if (Product.Value.GetType() == typeof(Energy))
            {
                totalMMbtu = ((Energy)Product.Value).MegabritishThermalUnits;
            }

            ICollection<Cost> costs = new List<Cost>();
            foreach (Cost cost in Costs)
            {
                if (cost.Value.GetType() == typeof(Money))
                {
                    Money totalCost = (Money)cost.Value;
                    double coeff = (double)totalCost.Amount / totalMMbtu;
                    Money partialCost = new Money { Amount = (decimal)(coeff * desiredMmbtu) };
                    Cost paCost = new Cost() { Name = cost.Name, Value = partialCost };
                    costs.Add(paCost);
                }

                if (cost.Value.GetType() == typeof(Mass))
                {
                    Mass totalCost = (Mass)cost.Value;
                    double coeff = (double)totalCost.Kilograms / totalMMbtu;
                    Mass partialCost = Mass.FromKilograms(coeff * desiredMmbtu);
                    Cost paCost = new Cost() { Name = cost.Name, Value = partialCost };
                    costs.Add(paCost);
                }
            }
            return costs;
        }
    }
}

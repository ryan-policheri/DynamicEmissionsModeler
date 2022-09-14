using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.Units;
using UnitsNet;

namespace EmissionsMonitorModel.ProcessModeling
{
    public abstract class ExchangeCostBase
    {
        public string CostName { get; set; }
        public abstract Type Type { get; }

        public ICollection<CostFactor> CostFactors { get; set; }

        public string FunctionCode { get;  set; }

        public dynamic FactorsToCostFunction { get; set; }
    }

    public class ExchangeCost<T> : ExchangeCostBase
    {
        public override Type Type => typeof(T);

        public CostTypes CostType { get; } = CostTypes.Custom;
    }

    public class MonetaryCost : ExchangeCost<Money>
    {
        public new CostTypes CostType { get; } = CostTypes.Monetary;
    }

    public class EmissionsCost : ExchangeCost<Mass>
    {
        public new CostTypes CostType { get; } = CostTypes.Emissions;
    }

    public enum CostTypes
    {
        Custom,
        Monetary,
        Emissions
    }

    public enum EmissionTypes
    {
        Undefined,
        CO2,
        NOx
    }
}

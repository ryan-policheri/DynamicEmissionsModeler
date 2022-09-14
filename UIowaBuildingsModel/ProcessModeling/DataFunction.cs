using System.ComponentModel.DataAnnotations.Schema;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.Units;
using UnitsNet;

namespace EmissionsMonitorModel.ProcessModeling
{
    public abstract class DataFunction
    {
        public string FunctionName { get; set; }

        public string FunctionCode { get; set; }

        public ICollection<FunctionFactor> FunctionFactors { get; set; }

        [NotMapped]
        public dynamic FunctionHostObject { get; set; }

        public string FunctionUnit { get; set; }

        public string FunctionUnitForm { get; set; }

        public DataFunctionResult ExecuteFunction(IEnumerable<DataPoint> functionFactorValues)
        {
            object obj = FunctionHostObject.Execute(functionFactorValues.First());
            double value = ToDefaultValueRendering(obj);
            return new DataFunctionResult()
            {
                Unit = FunctionUnit,
                UnitForm = FunctionUnitForm,
                TotalValue = value,
                ValueRenderFunction = this.ToDefaultValueRendering
            };
        }

        public abstract double ToDefaultValueRendering(object value);
    }

    public abstract class EnergyFunction : DataFunction
    {
        public EnergyFunction() : base()
        {
            FunctionUnit = "Energy";
        }
    }

    public class SteamEnergyFunction : EnergyFunction
    {
        public SteamEnergyFunction() : base()
        {
            FunctionUnitForm = "Steam";
        }

        public override double ToDefaultValueRendering(object value)
        {
            Energy energy = (Energy)value;
            return energy.MegabritishThermalUnits;
        }
    }

    public abstract class MassFunction : DataFunction
    {
        public MassFunction() : base()
        {
            FunctionUnit = "Mass";
        }
    }

    public class Co2MassFunction : MassFunction
    {
        public Co2MassFunction() : base()
        {
            FunctionUnitForm = "CO2";
        }

        public override double ToDefaultValueRendering(object value)
        {
            Mass mass = (Mass)value;
            return mass.Kilograms;
        }
    }

    public class MoneyFunction : DataFunction
    {
        public MoneyFunction() : base()
        {
            FunctionUnit = "Currency";
            FunctionUnitForm = "Money";
        }

        public override double ToDefaultValueRendering(object value)
        {
            Money money = (Money)value;
            return (double)money.Amount;
        }
    }

    public class FunctionFactor
    {
        public string FactorName { get; set; }

        public string FactorUri { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json.Serialization;
using DotNetCommon.DynamicCompilation;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.Units;
using UnitsNet;

namespace EmissionsMonitorModel.ProcessModeling
{
    //TODO: dynamically add/use GetAllFunctionTypeMappings()
    [JsonDerivedType(typeof(SteamEnergyFunction), "steam_energy")]
    [JsonDerivedType(typeof(MoneyFunction), "money_currency")]
    [JsonDerivedType(typeof(Co2MassFunction), "co2_mass")]
    public abstract class DataFunction
    {
        public DataFunction()
        {
            FunctionFactors = new List<FunctionFactor>();
        }

        public string FunctionName { get; set; }

        public string FunctionDescription { get; set; }

        public string FunctionCode { get; set; }

        public ICollection<FunctionFactor> FunctionFactors { get; set; }

        [NotMapped]
        public object FunctionHostObject { get; set; }

        public string FunctionUnit { get; set; }

        public string FunctionUnitForm { get; set; }

        public string FunctionDefaultReturnUnit { get; set; }

        public DataFunctionResult ExecuteFunction(IEnumerable<DataPoint> functionFactorValues)
        {
            Type type = this.FunctionHostObject.GetType();
            string methodName = this.FunctionName.ToValidMethodName();
            MethodInfo executeMethod = type.GetMethod(methodName);
            if (executeMethod == null) throw new InvalidOperationException($"Method {methodName} not found");

            var paramMap = functionFactorValues.Join(this.FunctionFactors, dp => dp.Series.SeriesUri.Uri, factor => factor.FactorUri.Uri,
                (dp, factor) => new
                {
                    SeriesName = dp.Series.SeriesUri.SeriesName,
                    FactorUri = factor.FactorUri,
                    ParameterName = factor.FactorName.ToValidVariableName() + "Point",
                    ParameterValue = dp,
                });
            if (paramMap.Count() != functionFactorValues.Count() || paramMap.Count() != this.FunctionFactors.Count()) throw new InvalidOperationException("The parameters are ill-formed");

            ParameterInfo[] parameters = executeMethod.GetParameters();
            ICollection<object> orderedArgs = new List<object>();
            foreach (ParameterInfo param in parameters.OrderBy(x => x.Position))
            {
                DataPoint correspondingDataPoint = paramMap.First(x => x.ParameterName == param.Name).ParameterValue;
                orderedArgs.Add(correspondingDataPoint);
            }

            object obj = executeMethod.Invoke(this.FunctionHostObject, orderedArgs.ToArray());
            double value = ToDefaultValueRendering(obj);

            return new DataFunctionResult()
            {
                FunctionName = this.FunctionName,
                UnitType = FunctionUnit,
                UnitForm = FunctionUnitForm,
                DefaultValueUnit = FunctionDefaultReturnUnit,
                TotalValue = value, 
                ValueRenderFunction = this.ToDefaultValueRendering
            };
        }

        public abstract Type GetReturnType();

        public abstract double ToDefaultValueRendering(object abstractValue);

        public FunctionTypeMapping ToTypeMapping() => 
            new FunctionTypeMapping { FunctionUnit = this.FunctionUnit, FunctionUnitForm = this.FunctionUnitForm, TypeRep = this.GetType(), BaseUnit = this.GetReturnType() };

        public static IEnumerable<FunctionTypeMapping> GetAllFunctionTypeMappings()
        {
            //TODO: don't hard code this. read from assembly instead
            yield return new SteamEnergyFunction().ToTypeMapping();
            yield return new Co2MassFunction().ToTypeMapping();
            yield return new MoneyFunction().ToTypeMapping();
        }

        public CSharpMethod ToCSharpMethod()
        {
            return new CSharpMethod
            {
                MethodName = this.FunctionName.ToValidMethodName(),
                ReturnType = this.GetReturnType(),
                MethodBody = this.FunctionCode,
                MethodParameters = this.FunctionFactors.Select(x => new CSharpMethodParameter
                {
                    ParameterName = x.ParameterName.ToValidVariableName(),
                    ParameterType = typeof(DataPoint)
                }).ToList()
            };
        }
    }

    public abstract class EnergyFunction : DataFunction
    {
        public EnergyFunction() : base()
        {
            FunctionUnit = "Energy";
        }

        public override Type GetReturnType() => typeof(Energy);
    }

    public class SteamEnergyFunction : EnergyFunction
    {
        public SteamEnergyFunction() : base()
        {
            FunctionUnitForm = "Steam";
            FunctionDefaultReturnUnit = "MMBTU";
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

        public override Type GetReturnType() => typeof(Mass);
    }

    public class Co2MassFunction : MassFunction
    {
        public Co2MassFunction() : base()
        {
            FunctionUnitForm = "CO2";
            FunctionDefaultReturnUnit = "Kilograms";
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
            FunctionDefaultReturnUnit = "";
        }

        public override Type GetReturnType() => typeof(Money);

        public override double ToDefaultValueRendering(object value)
        {
            Money money = (Money)value;
            return (double)money.Amount;
        }
    }

    public class FunctionFactor
    {
        public string FactorName { get; set; }

        public DataSourceSeriesUri FactorUri { get; set; }

        [NotMapped] 
        public string ParameterName => FactorName?.ToValidVariableName() + "Point";
    }

    public class FunctionTypeMapping
    {
        public string FunctionUnit { get; set; }

        public string FunctionUnitForm { get; set; }

        public Type TypeRep { get; set; }

        public Type BaseUnit { get; set; }
    }
}

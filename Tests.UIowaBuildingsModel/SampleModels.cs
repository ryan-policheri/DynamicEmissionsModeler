using System;
using System.Collections.Generic;
using System.Dynamic;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.Units;
using UIowaBuildingsModel.ConversionMethods;
using UnitsNet;

namespace Tests.EmissionsMonitorModel
{

    public static class SampleModels
    {
        private class Boiler1
        {
            public Func<DataPoint, Money> FuelCost => new Func<DataPoint, Money>((DataPoint fuelPoint) =>
            {
                Volume gasVolume = Volume.FromCubicFeet(fuelPoint.Value);
                decimal cost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet 
                return Money.FromUsDollars(cost);
            });

            public Func<DataPoint, Mass> EmissionsCost => new Func<DataPoint, Mass>((DataPoint fuelPoint) =>
            {
                Volume gasVolume = Volume.FromCubicFeet(fuelPoint.Value);
                Mass co2Emissions = NaturalGas.ToCo2Emissions(gasVolume);
                return co2Emissions;
            });

            public Func<DataPoint, Energy> SteamOutput => new Func<DataPoint, Energy>((DataPoint steamPoint) =>
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(steamPoint.Value);
                return steamEnergy;
            });
        }


        public static ExchangeNode BuildBoiler1ExchangeNode()
        {
            Boiler1 computeObject = new Boiler1();

            ExchangeNode node = new ExchangeNode
            {
                Name = "Boiler 1",
                Costs = new List<DataFunction>()
                {
                    new MoneyFunction()
                    {
                        FunctionName = "Fuel Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = "B1 NG flow tag" }
                        },
                        FunctionCode = "Foo",
                        FunctionHostObject = computeObject
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = "B1 NG flow tag"}
                        },
                        FunctionCode = "bar",
                        FunctionHostObject = computeObject
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam energy output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam output", FactorUri = "B1 Steam output tag" }
                    },
                    FunctionCode = "Foobar",
                    FunctionHostObject = computeObject
                }
            };

            return node;
        }

        public static ExchangeNode BuildBoiler2ExchangeNode()
        {
            dynamic computeObject1 = new ExpandoObject();
            computeObject1.Execute = new Func<DataPoint, DataPoint, Money>((DataPoint coalPoint, DataPoint naturalGasPoint) =>
            {
                Mass coalMass = Mass.FromPounds(coalPoint.Value);
                Volume gasVolume = Volume.FromCubicFeet(naturalGasPoint.Value);
                decimal coalCost = (decimal)(coalMass.Kilograms * 0.34); //34 cents per kg
                decimal gasCost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet 
                return Money.FromUsDollars(coalCost + gasCost);
            });

            dynamic computeObject2 = new ExpandoObject();
            computeObject2.Execute = new Func<DataPoint, DataPoint, Mass>((DataPoint coalPoint, DataPoint naturalGasPoint) =>
            {
                Mass coalMass = Mass.FromPounds(coalPoint.Value);
                Volume gasVolume = Volume.FromCubicFeet(naturalGasPoint.Value);
                Mass coalCo2Emissions = Coal.ToCo2Emissions(coalMass);
                Mass naturalGasCo2Emissions = NaturalGas.ToCo2Emissions(gasVolume);
                return coalCo2Emissions + naturalGasCo2Emissions;
            });

            dynamic computeObject3 = new ExpandoObject();
            computeObject3.Execute = new Func<DataPoint, Energy>((DataPoint fuelPoint) =>
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(fuelPoint.Value);
                return steamEnergy;
            });

            ExchangeNode node = new ExchangeNode
            {
                Name = "Boiler 2",
                Costs = new List<DataFunction>()
                {
                    new MoneyFunction()
                    {
                        FunctionName = "Fuel Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Coal Usage", FactorUri = "B2 Coal flow tag" },
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = "B2 NG flow tag" }
                        },
                        FunctionCode = "Foo",
                        FunctionHostObject = computeObject1
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Coal Usage", FactorUri = "B2 Coal flow tag" },
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = "B2 NG flow tag" }
                        },
                        FunctionCode = "bar",
                        FunctionHostObject = computeObject2
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam energy output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam output", FactorUri = "B2 Steam output tag" }
                    },
                    FunctionCode = "Foobar",
                    FunctionHostObject = computeObject3
                }
            };

            return node;
        }
    }
}

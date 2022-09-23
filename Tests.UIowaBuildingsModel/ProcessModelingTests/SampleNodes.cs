using System.Collections.Generic;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.Units;
using EmissionsMonitorModel.ConversionMethods;
using UnitsNet;
using EmissionsMonitorModel.DataSources;

namespace Tests.EmissionsMonitorModel.ProcessModelingTests
{
    public static class SampleNodes
    {
        public class Boiler1
        {
            public Money FuelCost(DataPoint NaturalGasUsagePoint)
            {
                Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);
                decimal cost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet 
                return Money.FromUsDollars(cost);
            }

            public Mass CO2EmissionsCost(DataPoint NaturalGasUsagePoint)
            {
                Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);
                Mass co2Emissions = NaturalGas.ToCo2Emissions(gasVolume);
                return co2Emissions;
            }

            public Energy SteamEnergyOutput(DataPoint SteamOutputPoint)
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(SteamOutputPoint.Value);
                return steamEnergy;
            }
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
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" }}
                        },
                        FunctionCode = "Foo",
                        FunctionHostObject = computeObject
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } }
                        },
                        FunctionCode = "bar",
                        FunctionHostObject = computeObject
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam Energy Output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam Output", FactorUri =  new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" } }
                    },
                    FunctionCode = "Foobar",
                    FunctionHostObject = computeObject
                }
            };

            return node;
        }

        public class Boiler2
        {
            public Money FuelCost(DataPoint CoalUsagePoint, DataPoint NaturalGasUsagePoint)
            {
                Mass coalMass = Mass.FromPounds(CoalUsagePoint.Value);
                Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);
                decimal coalCost = (decimal)(coalMass.Kilograms * 0.34); //34 cents per kg
                decimal gasCost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet 
                return Money.FromUsDollars(coalCost + gasCost);
            }

            public Mass CO2EmissionsCost(DataPoint CoalUsagePoint, DataPoint NaturalGasUsagePoint)
            {
                Mass coalMass = Mass.FromPounds(CoalUsagePoint.Value);
                Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);
                Mass coalCo2Emissions = Coal.ToCo2Emissions(coalMass);
                Mass naturalGasCo2Emissions = NaturalGas.ToCo2Emissions(gasVolume);
                return coalCo2Emissions + naturalGasCo2Emissions;
            }

            public Energy SteamEnergyOutput(DataPoint SteamOutputPoint)
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(SteamOutputPoint.Value);
                return steamEnergy;
            }
        }

        public static ExchangeNode BuildBoiler2ExchangeNode()
        {
            Boiler2 boiler2Compute = new Boiler2();

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
                            new FunctionFactor() { FactorName = "Coal Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 Coal Flow Tag" } },
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 NG Flow Tag" } }
                        },
                        FunctionCode = "Foo",
                        FunctionHostObject = boiler2Compute
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Coal Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 Coal Flow Tag" } },
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 NG Flow Tag" } }
                        },
                        FunctionCode = "bar",
                        FunctionHostObject = boiler2Compute
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam Energy Output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam Output", FactorUri = new DataSourceSeriesUri { Uri = "B2 Steam Output Tag" } }
                    },
                    FunctionCode = "Foobar",
                    FunctionHostObject = boiler2Compute
                }
            };

            return node;
        }

        public static LikeTermsAggregatorNode BuildBoiler_1_2_TermAggregator()
        {
            LikeTermsAggregatorNode node = new LikeTermsAggregatorNode();

            node.PrecedingNodes = new List<ExchangeNode>()
            {
                BuildBoiler1ExchangeNode(),
                BuildBoiler2ExchangeNode()
            };

            return node;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitsNet;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorModel.Units;
using FluentAssertions;
using UIowaBuildingsModel.ConversionMethods;

namespace Tests.EmissionsMonitorModel
{
    [TestClass]
    public class ProcessModelTests
    {
        [TestMethod]
        public void WhenSettingUpBasicExchangeNode_ShouldCorrectlyCalculateOutputCosts()
        {
            dynamic computeObject1 = new ExpandoObject();
            computeObject1.Execute = new Func<DataPoint, Money>((DataPoint fuelPoint) =>
            {
                Volume gasVolume = Volume.FromCubicFeet(fuelPoint.Value);
                decimal cost = (decimal) (gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet 
                return Money.FromUsDollars(cost);
            });

            dynamic computeObject2 = new ExpandoObject();
            computeObject2.Execute = new Func<DataPoint, Mass>((DataPoint fuelPoint) =>
            {
                Volume gasVolume = Volume.FromCubicFeet(fuelPoint.Value);
                Mass co2Emissions = NaturalGas.ToCo2Emissions(gasVolume);
                return co2Emissions;
            });

            dynamic computeObject3 = new ExpandoObject();
            computeObject3.Execute = new Func<DataPoint, Energy>((DataPoint fuelPoint) =>
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(fuelPoint.Value);
                return steamEnergy;
            });

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
                        FunctionHostObject = computeObject1
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = "B1 NG flow tag"}
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
                        new FunctionFactor() { FactorName = "Steam output", FactorUri = "B1 Steam output tag" }
                    },
                    FunctionCode = "Foobar",
                    FunctionHostObject = computeObject3
                }
            };

            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint() { SeriesName = "B1 NG flow tag", Value = 5000});
            dataPoints.Add(new DataPoint() { SeriesName = "B1 Steam output tag", Value = 20});
            
            ProductCostResults results = node.RenderProductAndCosts(dataPoints);
            ICollection<Cost> costs = results.CalculateCostOfProductAmount(Energy.FromMegabritishThermalUnits(2));
            double money = costs.First(x => x.Name == "Money Currency").Value;
            money.Should().Be(5);
            double co2 = costs.First(x => x.Name == "CO2 Mass").Value;
            co2.Should().Be(27.55);
        }

        //HELPERS

    }
}

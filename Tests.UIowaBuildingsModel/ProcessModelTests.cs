using System;
using System.Collections;
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
                Costs = new List<ExchangeCostBase>()
                {
                    new MonetaryCost()
                    {
                        CostName = "Fuel Cost",
                        CostFactors = new List<CostFactor>()
                        {
                            new CostFactor() { FactorType = "Natural Gas Usage", FactorUri = "B1 NG flow tag" }
                        },
                        FunctionCode = "Foo",
                        FactorsToCostFunction = computeObject1
                    },
                    new EmissionsCost()
                    {
                        CostName = "CO2 Emissions Cost",
                        CostFactors = new List<CostFactor>()
                        {
                            new CostFactor() { FactorType = "Natural Gas Usage", FactorUri = "B1 NG flow tag"}
                        },
                        FunctionCode = "bar",
                        FactorsToCostFunction = computeObject2
                    }
                },
                Product = new ExchangeCost<Energy>()
                {
                    CostName = "Steam energy output",
                    CostFactors = new List<CostFactor>()
                    {
                        new CostFactor() { FactorType = "Steam output", FactorUri = "B1 Steam output tag" }
                    },
                    FunctionCode = "Foobar",
                    FactorsToCostFunction = computeObject3
                }
            };

            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint() { SeriesName = "B1 NG flow tag", Value = 5000});
            dataPoints.Add(new DataPoint() { SeriesName = "B1 Steam output tag", Value = 20});
            
            ProductCostConverter converter = node.RenderProductAndCosts(dataPoints);
            ICollection<Cost> costs = converter.CalculateCostOfProductAmount(Energy.FromMegabritishThermalUnits(2));
            Money money = (Money)costs.First(x => x.Name == "Fuel Cost").Value;
            money.Amount.Should().Be(5m);
            Mass co2 = (Mass)costs.First(x => x.Name == "CO2 Emissions Cost").Value;
            co2.Kilograms.Should().Be(27.55);
        }

        //HELPERS

    }
}

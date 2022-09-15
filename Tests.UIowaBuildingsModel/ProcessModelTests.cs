using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitsNet;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using FluentAssertions;

namespace Tests.EmissionsMonitorModel
{
    [TestClass]
    public class ProcessModelTests
    {
        [TestMethod]
        public void WhenSettingUpBasicExchangeNode_ShouldCorrectlyCalculateProductCosts()
        {
            //MODEL
            ExchangeNode boiler1ExchangeNode = SampleModels.BuildBoiler1ExchangeNode();

            //DATA
            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint() { SeriesName = "B1 NG Flow Tag", Value = 5000});
            dataPoints.Add(new DataPoint() { SeriesName = "B1 Steam Output Tag", Value = 20});

            ProductCostResults results = boiler1ExchangeNode.RenderProductAndCosts(dataPoints);
            ICollection<Cost> costs = results.CalculateCostOfProductAmount(Energy.FromMegabritishThermalUnits(2));
            double money = costs.First(x => x.Name == "Money Currency").Value;
            money.Should().Be(5);
            double co2 = costs.First(x => x.Name == "CO2 Mass").Value;
            co2.Should().Be(27.55);
        }

        [TestMethod]
        public void WhenSettingUpExchangeNodeWithMultiFactorFunction_ShouldCorrectlyCalculateProductCosts()
        {
            //MODEL
            ExchangeNode boiler2ExchangeNode = SampleModels.BuildBoiler2ExchangeNode();

            //DATA
            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint() { SeriesName = "B2 Coal Flow Tag", Value = 500 }); //$77.11064 of coal, 1035 pounds of CO2
            dataPoints.Add(new DataPoint() { SeriesName = "B2 NG Flow Tag", Value = 2500 }); //$25 of NG, 137.75 Kgs of CO2
            dataPoints.Add(new DataPoint() { SeriesName = "B2 Steam Output Tag", Value = 25 }); //25 MMBTU

            //TOTAL COST: $77.11064 + $25 = $102.11064 
            //TOTAL CO2: 1035 pounds + 137.75 Kgs = 469.4681 Kgs + 137.75 Kgs = 607.2181 Kgs
            //COST OF 5 MMBTU: 0.20 * $102.11064 = $20.422128
            //CO2 OF 5 MMBTU: 0.20 * 607.2181 = 121.44362

            ProductCostResults results = boiler2ExchangeNode.RenderProductAndCosts(dataPoints);
            ICollection<Cost> costs = results.CalculateCostOfProductAmount(Energy.FromMegabritishThermalUnits(5));
            double money = costs.First(x => x.Name == "Money Currency").Value;
            Math.Round(money, 2).Should().Be(20.42);
            double co2 = costs.First(x => x.Name == "CO2 Mass").Value;
            Math.Round(co2, 2).Should().Be(121.44);
        }
    }
}

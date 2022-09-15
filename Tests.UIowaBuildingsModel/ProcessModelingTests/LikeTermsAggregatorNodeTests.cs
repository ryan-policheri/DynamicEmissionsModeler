using System;
using System.Collections.Generic;
using System.Linq;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitsNet;

namespace Tests.EmissionsMonitorModel.ProcessModelingTests
{
    [TestClass]
    public class LikeTermsAggregatorNodeTests
    {
        [TestMethod]
        public void WhenAggregatingTwoExchangeNodes_ProportionallyCombinesLikeTerms()
        {
            LikeTermsAggregatorNode node = SampleModels.BuildBoiler_1_2_TermAggregator();

            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint() { SeriesName = "B1 NG Flow Tag", Value = 5000 }); //$50 dollars of natural gas, 275.5 kgs of CO2
            dataPoints.Add(new DataPoint() { SeriesName = "B1 Steam Output Tag", Value = 20 }); //20 MMBTU
            dataPoints.Add(new DataPoint() { SeriesName = "B2 Coal Flow Tag", Value = 500 }); //$77.11064 of coal, 1035 pounds of CO2
            dataPoints.Add(new DataPoint() { SeriesName = "B2 NG Flow Tag", Value = 2500 }); //$25 of NG, 137.75 Kgs of CO2
            dataPoints.Add(new DataPoint() { SeriesName = "B2 Steam Output Tag", Value = 25 }); //25 MMBTU

            ProductCostResults results = node.RenderProductAndCosts(dataPoints);
            ICollection<Cost> costs = results.CalculateCostOfProductAmount(Energy.FromMegabritishThermalUnits(5));

            //BOILER 1 TOTAL COST: $50
            //BOILER 1 TOTAL CO2: 275.5 KG
            //BOILER 2 TOTAL COST: $77.11064 + $25 = $102.11064 
            //BOILER 2 TOTAL CO2: 1035 pounds + 137.75 Kgs = 469.4681 Kgs + 137.75 Kgs = 607.2181 Kgs

            //COMBINED TOTAL COST = $50 + $102.11064 = $152.11064
            //COMBINED TOTAL CO2 = 275.5 Kgs + 607.2181 Kgs = 882.7181 kgs
            //COMBINED TOTAL OUTPUT = 45 MMBTU

            //PARTIAL COST = 0.111111 * $152.11064 = $16.9011653
            //PARTIAL CO2 = 0.111111 * 882.7181 kgs = 98.0796908091

            double money = costs.First(x => x.Name == "Money Currency").Value;
            Math.Round(money, 2).Should().Be(16.90);
            double co2 = costs.First(x => x.Name == "CO2 Mass").Value;
            Math.Round(co2, 2).Should().Be(98.08);
        }
    }
}

using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace Tests.EmissionsMonitorModel.ProcessModelingTests
{
    [TestClass]
    public class StreamSplitterNodeTests
    {
        public class StreamSplitter
        {
            public Energy SteamForTurbine(DataPoint TurbineSteamInputPoint)
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(TurbineSteamInputPoint.Value);
                return steamEnergy;
            }
        }

        [TestMethod]
        public void WhenGivenPreceedingNodeWithProductAndCost_CorrectlySplitsIntoTwoStream()
        {
            //MODEL
            ExchangeNode boiler1ExchangeNode = SampleNodes.BuildBoiler1ExchangeNode();
            StreamSplitterNode streamSplitterNode = new StreamSplitterNode()
            {
                PrecedingNode = boiler1ExchangeNode,
                SplitFunction = new SteamEnergyFunction()
                {
                    FunctionName = "Steam For Turbine",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Turbine Steam Input", FactorUri = new DataSourceSeriesUri { Uri = "Turbine Steam Input Tag" } }
                    },
                    FunctionCode = "Foobar",
                    FunctionHostObject = new StreamSplitter(),
                }
            };

            //DATA
            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } },
                Value = 5000 //$50 dollars of natural gas, 275.5 kgs of CO2
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" } },
                Value = 20 //20 MMBTU
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Turbine Steam Input Tag" } },
                Value = 5 //5MMBTU
            });

            //TOTAL PRODUCT = 20 MMBTU
            //TOTAL COST: $50
            //TOTAL CO2: 275.5 KG       

            //TURBINE PRODUCT STREAM = 5 MMBTU
            //TURBINE COST STREAM = $12.5
            //TURBINE CO2 STREAM = 68.875 KG

            //REMAINDER PRODUCT STREAM = 15 MMBTU
            //REMAINDER COST STREAM = $37.5
            //REMAINDER CO2 STREAM = 206.625 KG

            ProductCostResults turbineStream = streamSplitterNode.RenderSplitFunctionProductAndCosts(dataPoints);
            ProductCostResults remainderStream = streamSplitterNode.RenderRemainderProductAndCost(dataPoints);

            turbineStream.Product.TotalValue.Should().Be(5);
            turbineStream.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(12.5);
            turbineStream.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(68.875);

            remainderStream.Product.TotalValue.Should().Be(15);
            remainderStream.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(37.5);
            remainderStream.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(206.625);
        }
    }
}

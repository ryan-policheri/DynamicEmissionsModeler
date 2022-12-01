using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace Tests.EmissionsMonitorModel.ProcessModelingTests
{
    [TestClass]
    public class MultiSplitterNodeTests
    {
        public class SplitFunctions
        {
            public Energy SteamForPlant1(DataPoint Plant1InputPoint)
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(Plant1InputPoint.Value);
                return steamEnergy;
            }

            public Energy SteamForPlant2(DataPoint Plant2InputPoint)
            {
                Energy steamEnergy = Energy.FromMegabritishThermalUnits(Plant2InputPoint.Value);
                return steamEnergy;
            }
        }

        [TestMethod]
        public void WhenStreamIsSplitByMultipleFunctions_EachSplitIsCorrectAndRemainderIsCorrect()
        {
            //MODEL
            ExchangeNode boiler1ExchangeNode = SampleNodes.BuildBoiler1ExchangeNode();
            MultiSplitterNode multiSplitterNode = BuildMultiSplitterNode(boiler1ExchangeNode);

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
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Plant 1 Steam Input Tag" } },
                Value = 5 //5 MMBTU
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Plant 2 Steam Input Tag" } },
                Value = 12.5 //12.5 MMBTU
            });

            //TOTAL PRODUCT = 20 MMBTU
            //TOTAL COST: $50
            //TOTAL CO2: 275.5 KG       

            //SPLIT 1 PRODUCT STREAM = 5 MMBTU
            //SPLIT 1 COST STREAM = $12.5
            //SPLIT 1 CO2 STREAM = 68.875 KG

            //SPLIT 2 PRODUCT STREAM = 12.5 MMBTU
            //SPLIT 2 COST STREAM = $31.25
            //SPLIT 2 CO2 STREAM = 172.1875 KG

            //REMAINDER PRODUCT STREAM = 2.5 MMBTU
            //REMAINDER COST STREAM = $6.25
            //REMAINDER CO2 STREAM = 34.4375 KG

            var split1Steam = multiSplitterNode.SplitResultNodes.First(x => x.Name == "Steam For Plant 1 (From Multi Split)").RenderProductAndCosts(dataPoints);
            var split2Steam = multiSplitterNode.SplitResultNodes.First(x => x.Name == "Steam For Plant 2 (From Multi Split)").RenderProductAndCosts(dataPoints);

            split1Steam.Product.TotalValue.Should().Be(5);
            split1Steam.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(12.5);
            split1Steam.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(68.875);

            split2Steam.Product.TotalValue.Should().Be(12.5);
            split2Steam.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(31.25);
            split2Steam.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(172.1875);

            var remainder = multiSplitterNode.RemainderResultNode.RenderProductAndCosts(dataPoints);
            remainder.Product.TotalValue.Should().Be(2.5);
            remainder.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(6.25);
            remainder.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(34.4375);
        }

        private MultiSplitterNode BuildMultiSplitterNode(ExchangeNode nodeToSplit)
        {
            var node = new MultiSplitterNode()
            {
                PrecedingNode = nodeToSplit,
                SplitFunctions = new List<DataFunction>()
            };
            node.AddSplitFunction(new SteamEnergyFunction()
            {
                FunctionName = "Steam For Plant 1",
                FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Plant 1 Input", FactorUri = new DataSourceSeriesUri { Uri = "Plant 1 Steam Input Tag" }}
                        },
                FunctionCode = "Foo",
                FunctionHostObject = new SplitFunctions()
            });
            node.AddSplitFunction(new SteamEnergyFunction()
            {
                FunctionName = "Steam For Plant 2",
                FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Plant 2 Input", FactorUri = new DataSourceSeriesUri { Uri = "Plant 2 Steam Input Tag" } }
                        },
                FunctionCode = "bar",
                FunctionHostObject = new SplitFunctions()
            });
            return node;
        }
    }
}

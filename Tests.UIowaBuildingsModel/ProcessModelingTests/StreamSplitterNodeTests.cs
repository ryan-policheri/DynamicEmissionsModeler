using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using static Tests.EmissionsMonitorModel.ProcessModelingTests.ProductConversionNodeTests;

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
            StreamSplitterNode streamSplitterNode = BuildSplitterNode(boiler1ExchangeNode);

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

            Action<ProductCostResults> checkSplitStream = (splitStreamResults) =>
            {
                splitStreamResults.Product.TotalValue.Should().Be(5);
                splitStreamResults.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(12.5);
                splitStreamResults.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(68.875);
            };

            ProductCostResults turbineStream = streamSplitterNode.RenderSplitFunctionProductAndCosts(dataPoints);
            checkSplitStream(turbineStream);
            turbineStream = streamSplitterNode.SplitResultNode.RenderProductAndCosts(dataPoints);
            checkSplitStream(turbineStream);

            Action<ProductCostResults> checkRemainderStream = (remainderStreamResults) =>
            {
                remainderStreamResults.Product.TotalValue.Should().Be(15);
                remainderStreamResults.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(37.5);
                remainderStreamResults.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(206.625);
            };

            ProductCostResults remainderStream = streamSplitterNode.RenderRemainderProductAndCost(dataPoints);
            checkRemainderStream(remainderStream);
            remainderStream = streamSplitterNode.RemainderResultNode.RenderProductAndCosts(dataPoints);
        }

        [TestMethod]
        public void WhenPreceedingNodeIsSplitStreamResult_UsingStreamResultWorks()
        {
            //MODEL
            ExchangeNode boiler1ExchangeNode = SampleNodes.BuildBoiler1ExchangeNode();
            StreamSplitterNode streamSplitterNode = BuildSplitterNode(boiler1ExchangeNode);
            ProductConversionNode conversionNode = new ProductConversionNode 
            { 
                PrecedingNode = streamSplitterNode.SplitResultNode,
                NewProductFunction = new ElectricEnergyFunction()
                {
                    FunctionName = "Steam To Electric",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Electric Generation", FactorUri = new DataSourceSeriesUri { Uri = "Turbine Generation Tag" } }
                    },
                    FunctionCode = "Foobar",
                    FunctionHostObject = new ProductConversion(),
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
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Turbine Generation Tag" } },
                Value = 60 //MJ
            });

            ProductCostResults results = conversionNode.RenderProductAndCosts(dataPoints);

            //TOTALS
            results.Product.TotalValue.Should().Be(60);
            results.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(12.5);
            results.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(68.875);

            //PER UNIT
            var costsPerUnit = results.CalculateCostOfAbstractProductAmount(Energy.FromMegajoules(1));
            double money = costsPerUnit.First(x => x.Name == "Money Currency").Value;
            Math.Round(money, 3).Should().Be(0.208);
            double co2 = costsPerUnit.First(x => x.Name == "CO2 Mass").Value;
            Math.Round(co2, 3).Should().Be(1.148);
        }

        private StreamSplitterNode BuildSplitterNode(ExchangeNode nodeToSplit)
        {
            return new StreamSplitterNode()
            {
                PrecedingNode = nodeToSplit,
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
        }
    }
}

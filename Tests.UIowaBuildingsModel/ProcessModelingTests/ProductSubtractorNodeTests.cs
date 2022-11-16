using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace Tests.EmissionsMonitorModel.ProcessModelingTests
{
    [TestClass]
    public class ProductSubtractorNodeTests
    {
        public class ProductDeductionFunctionHost
        {
            public Energy ProductDeduction(DataPoint OverheadPoint)
            {
                Energy overhead = Energy.FromMegabritishThermalUnits(OverheadPoint.Value);
                return overhead;
            }
        }

        [TestMethod]
        public void WhenSubtractingProduct_CostsGoUpProportionally()
        {
            //MODEL
            ExchangeNode boiler1ExchangeNode = SampleNodes.BuildBoiler1ExchangeNode();
            ProductSubtractorNode deducterNode = new ProductSubtractorNode();
            deducterNode.PrecedingNode = boiler1ExchangeNode;
            deducterNode.ProductDeductionFunction = new SteamEnergyFunction()
            {
                FunctionName = "Product Deduction",
                FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Overhead", FactorUri = new DataSourceSeriesUri { Uri = "Overhead Tag" } }
                    },
                FunctionCode = "Foobar",
                FunctionHostObject = new ProductDeductionFunctionHost(),
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
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Overhead Tag" } },
                Value = 5 // MMBTU
            });

            ProductCostResults deductedResults = deducterNode.RenderProductAndCosts(dataPoints);

            deductedResults.Product.TotalValue.Should().Be(15);
            deductedResults.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(50);
            deductedResults.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(275.5);

            var perUnitCostsForDeduction = deductedResults.CalculateCostOfAbstractProductAmount(Energy.FromMegabritishThermalUnits(1));
            Math.Round(perUnitCostsForDeduction.First(x => x.CostFunctionName == "Fuel Cost").Value, 3).Should().Be(3.333);
            Math.Round(perUnitCostsForDeduction.First(x => x.CostFunctionName == "CO2 Emissions Cost").Value, 3).Should().Be(18.367);

            //Make sure original is not impacted
            ProductCostResults originalResults = boiler1ExchangeNode.RenderProductAndCosts(dataPoints);
            originalResults.Product.TotalValue.Should().Be(20);
            var perUnitCostsForOriginal = originalResults.CalculateCostOfAbstractProductAmount(Energy.FromMegabritishThermalUnits(1));
            Math.Round(perUnitCostsForOriginal.First(x => x.CostFunctionName == "Fuel Cost").Value, 1).Should().Be(2.5);
            Math.Round(perUnitCostsForOriginal.First(x => x.CostFunctionName == "CO2 Emissions Cost").Value, 3).Should().Be(13.775);
        }
    }
}

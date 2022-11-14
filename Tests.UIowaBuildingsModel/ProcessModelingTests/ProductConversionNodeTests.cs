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
using static Tests.EmissionsMonitorModel.ProcessModelingTests.StreamSplitterNodeTests;

namespace Tests.EmissionsMonitorModel.ProcessModelingTests
{
    [TestClass]
    public class ProductConversionNodeTests
    {
        public class ProductConversion
        {
            public Energy SteamToElectric(DataPoint ElectricGenerationPoint)
            {
                Energy electricEnergy = Energy.FromMegajoules(ElectricGenerationPoint.Value);
                return electricEnergy;
            }
        }

        [TestMethod]
        public void WhenGivenPreceedingExchangeNode_CorrectlyKeepsCostsWithNewProduct()
        {
            //MODEL
            ExchangeNode boiler1ExchangeNode = SampleNodes.BuildBoiler1ExchangeNode();
            ProductConversionNode conversionNode = new ProductConversionNode
            {
                PrecedingNode = boiler1ExchangeNode,
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
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Turbine Generation Tag" } },
                Value = 60 //MJ
            });

            ProductCostResults results = conversionNode.RenderProductAndCosts(dataPoints);

            //TOTALS
            results.Product.TotalValue.Should().Be(60);
            results.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(50);
            results.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(275.5);

            var costsPerUnit = results.CalculateCostOfAbstractProductAmount(Energy.FromMegajoules(1));
            double money = costsPerUnit.First(x => x.Name == "Money Currency").Value;
            Math.Round(money, 3).Should().Be(0.833);
            double co2 = costsPerUnit.First(x => x.Name == "CO2 Mass").Value;
            Math.Round(co2, 3).Should().Be(4.592);
        }
    }
}

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
    public class MultiProductConversionNodeTests
    {
        public class ProductConversion
        {
            public Volume ElectricAndSteamToChilledWater(DataPoint ChilledWaterProducedPoint)
            {
                Volume chilledWater = Volume.FromUsGallons(ChilledWaterProducedPoint.Value);
                return chilledWater;
            }
        }

        [TestMethod]
        public void WhenConvertingMultipleProductsIntoANewProduct_CostsAreCorrectlyTransferredToTheNewProduct()
        {
            //MODEL
            var steamExchangeNode = SampleNodes.BuildBoiler1ExchangeNode();
            var electricExchangeNode = SampleNodes.BuildElectricGeneratorExchangeNode();

            MultiProductConversionNode multiConversionNode = new MultiProductConversionNode
            {
                PrecedingNodes = new List<ProcessNode> { steamExchangeNode, electricExchangeNode },
                NewProductFunction = new ChilledWaterVolumeFunction()
                {
                    FunctionName = "Electric And Steam To Chilled Water",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Chilled Water Produced", FactorUri = new DataSourceSeriesUri { Uri = "Water Prod Tag" } }
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
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Electric Gen NG Flow Tag" } },
                Value = 22800 //$228 dollars of natural gas, 1256.28 kgs of CO2
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Electric Output Tag" } },
                Value = 3 //3 MWH
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Water Prod Tag" } },
                Value = 1000 //1000 U.S. Gallons
            });

            ProductCostResults results = multiConversionNode.RenderProductAndCosts(dataPoints);

            //TOTALS
            results.Product.TotalValue.Should().Be(1000);
            results.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(278);
            results.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(1531.78);

            //PER UNIT
            var costsPerUnit = results.CalculateCostOfAbstractProductAmount(Volume.FromUsGallons(1));
            double money = costsPerUnit.First(x => x.Name == "Money Currency").Value;
            Math.Round(money, 3).Should().Be(0.278);
            double co2 = costsPerUnit.First(x => x.Name == "CO2 Mass").Value;
            Math.Round(co2, 3).Should().Be(1.532);
        }
    }
}

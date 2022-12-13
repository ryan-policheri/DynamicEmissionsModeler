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
using static Tests.EmissionsMonitorModel.ProcessModelingTests.ProductConversionNodeTests;

namespace Tests.EmissionsMonitorModel.ProcessModelingTests
{
    [TestClass]
    public class UsageAdjusterNodeTests
    {
        public class AdjusterFunctionHost
        {
            public Energy ActualUsage(DataPoint ActualUsagePoint)
            {
                Energy actualUsage = Energy.FromMegabritishThermalUnits(ActualUsagePoint.Value);
                return actualUsage;
            }
        }

        [TestMethod]
        public void WhenAdjustingActualUsage_ProductAndCostsReflectActualUsage()
        {
            //MODEL
            ExchangeNode boiler1ExchangeNode = SampleNodes.BuildBoiler1ExchangeNode();
            UsageAdjusterNode adjusterNode = new UsageAdjusterNode();
            adjusterNode.PrecedingNode = boiler1ExchangeNode;
            adjusterNode.ProductUsageFunction = new SteamEnergyFunction()
            {
                FunctionName = "Actual Usage",
                FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Actual Usage", FactorUri = new DataSourceSeriesUri { Uri = "Actual Usage Tag" } }
                    },
                FunctionCode = "Foobar",
                FunctionHostObject = new AdjusterFunctionHost(),
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
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "Actual Usage Tag" } },
                Value = 5 // MMBTU
            });

            ProductCostResults results = adjusterNode.RenderProductAndCosts(dataPoints);

            results.Product.TotalValue.Should().Be(5);
            results.Costs.First(x => x.UnitForm == "Money").TotalValue.Should().Be(12.5);
            results.Costs.First(x => x.UnitForm == "CO2").TotalValue.Should().Be(68.875);
        }
    }
}

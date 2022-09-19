using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCommon;
using DotNetCommon.Helpers;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorServices;
using FluentAssertions;
using Moq;
using Tests.EmissionsMonitorModel.ProcessModelingTests;

namespace Tests.EmissionsMonitorServices
{
    [TestClass]
    public class ModelExecutionServiceTests
    {
        [TestMethod]
        public async Task WhenGivenExecuteSpecOfSingleNodeModel_CalculatesTimeSeriesData()
        {
            ITimeSeriesDataSource mockSource = DataSourceMock().Object;
            ModelExecutionService service = new ModelExecutionService(mockSource);
            ModelExecutionSpec spec = new ModelExecutionSpec();

            ProcessModel model = new ProcessModel();
            model.ModelName = "TestModel";
            model.AddProcessNode(SampleNodes.BuildBoiler1ExchangeNode());
            spec.Model = model;
            spec.StartTime = new DateTimeOffset(2022, 9, 18, 6, 5, 4, TimeZones.GetUtcOffset());
            spec.EndTime = new DateTimeOffset(2022, 9, 18, 6, 5, 13, TimeZones.GetUtcOffset());

            ICollection<MonitorSeries> monitorSeries = await service.ExecuteModelAsync(spec);

            monitorSeries.First().DataPoints.ElementAt(0).Values.Product.TotalValue.Should().Be(1);
            monitorSeries.First().DataPoints.ElementAt(2).Values.Product.TotalValue.Should().Be(3);
            monitorSeries.First().DataPoints.ElementAt(6).Values.Product.TotalValue.Should().Be(2.5);

            monitorSeries.First().DataPoints.ElementAt(0).Values.Costs.First(x => x.Unit == "Currency").TotalValue.Should().Be(0.2);
            monitorSeries.First().DataPoints.ElementAt(2).Values.Costs.First(x => x.Unit == "Currency").TotalValue.Should().Be(0.3);
            monitorSeries.First().DataPoints.ElementAt(6).Values.Costs.First(x => x.Unit == "Currency").TotalValue.Should().Be(0.25);

            monitorSeries.First().DataPoints.ElementAt(0).Values.Costs.First(x => x.Unit == "Mass").TotalValue.Should().Be(1.102);
            monitorSeries.First().DataPoints.ElementAt(2).Values.Costs.First(x => x.Unit == "Mass").TotalValue.Should().Be(1.653);
            Math.Round(monitorSeries.First().DataPoints.ElementAt(6).Values.Costs.First(x => x.Unit == "Mass").TotalValue, 4).Should().Be(1.3775);
        }

        private Mock<ITimeSeriesDataSource> DataSourceMock()
        {
            Mock<ITimeSeriesDataSource> dataSource = new Mock<ITimeSeriesDataSource>();
            dataSource.Setup(x => x.GetTimeSeriesAsync("B1 NG Flow Tag", It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()).Result)
                .Returns(() => new Series
                {
                    SeriesName = "B1 NG Flow Tag",
                    DataPoints = new List<DataPoint>()
                    {
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 4, TimeZones.GetUtcOffset()), Value = 20 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 5, TimeZones.GetUtcOffset()), Value = 25 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 6, TimeZones.GetUtcOffset()), Value = 30 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 7, TimeZones.GetUtcOffset()), Value = 25 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 8, TimeZones.GetUtcOffset()), Value = 20 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 9, TimeZones.GetUtcOffset()), Value = 15 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 10, TimeZones.GetUtcOffset()), Value = 25 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 11, TimeZones.GetUtcOffset()), Value = 25 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 12, TimeZones.GetUtcOffset()), Value = 30 },
                        new DataPoint { SeriesName = "B1 NG Flow Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 13, TimeZones.GetUtcOffset()), Value = 25 }
                    }
                });
            dataSource.Setup(x => x.GetTimeSeriesAsync("B1 Steam Output Tag", It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()).Result)
                .Returns(() => new Series
                {
                    SeriesName = "B1 Steam Output Tag",
                    DataPoints = new List<DataPoint>()
                    {
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 4, TimeZones.GetUtcOffset()), Value = 1 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 5, TimeZones.GetUtcOffset()), Value = 2 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 6, TimeZones.GetUtcOffset()), Value = 3 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 7, TimeZones.GetUtcOffset()), Value = 2 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 8, TimeZones.GetUtcOffset()), Value = 1 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 9, TimeZones.GetUtcOffset()), Value = 2 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 10, TimeZones.GetUtcOffset()), Value = 2.5 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 11, TimeZones.GetUtcOffset()), Value = 3 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 12, TimeZones.GetUtcOffset()), Value = 3.5 },
                        new DataPoint { SeriesName = "B1 Steam Output Tag", Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 13, TimeZones.GetUtcOffset()), Value = 3 }
                    }
                });
            return dataSource;
        }

        private ProcessModel SingleNodeProcessModel()
        {
            ExchangeNode node = new ExchangeNode
            {
                Name = "Boiler 1",
                Costs = new List<DataFunction>()
                {
                    new MoneyFunction()
                    {
                        FunctionName = "Fuel Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = "B1 NG Flow Tag" }
                        },
                        FunctionCode = $"Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}decimal cost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet{Environment.NewLine}return Money.FromUsDollars(cost);",
                        FunctionHostObject = null
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = "B1 NG Flow Tag"}
                        },
                        FunctionCode = $"Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}Mass co2Emissions = NaturalGas.ToCo2Emissions(gasVolume);{Environment.NewLine}return co2Emissions;",
                        FunctionHostObject = null
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam Energy Output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam Output", FactorUri = "B1 Steam Output Tag" }
                    },
                    FunctionCode = $"Energy steamEnergy = Energy.FromMegabritishThermalUnits(SteamOutputPoint.Value);{Environment.NewLine}return steamEnergy;",
                    FunctionHostObject = null
                }
            };


            ProcessModel model = new ProcessModel();
            model.ModelName = "TestModel";
            model.AddProcessNode(node);
            return model;
        }
    }
}

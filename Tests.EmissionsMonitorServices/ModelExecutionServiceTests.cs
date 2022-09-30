﻿using DotNetCommon.Helpers;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorDataAccess;
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
            DataSourceBase mockDataSource = new DataSourceBase
            {
                SourceId = 1,
                SourceName = "Fake Data Source"
            };

            DataSourceSeriesUri mockGasSeries = new DataSourceSeriesUri
            {
                DataSource = mockDataSource,
                SeriesName = "Boiler 1 Natural Gas Flow",
                Prefix = "Foobar",
                Uri = "B1 NG Flow Tag"
            };

            DataSourceSeriesUri mockSteamSeries = new DataSourceSeriesUri
            {
                DataSource = mockDataSource,
                SeriesName = "Boiler 1 Steam Energy Flow",
                Prefix = "Foobar",
                Uri = "B1 Steam Output Tag"
            };

            Mock<ITimeSeriesDataSource> dataSource = new Mock<ITimeSeriesDataSource>();
            dataSource.Setup(x => x.GetTimeSeriesAsync(It.Is<DataSourceSeriesUri>(x => x.Uri == "B1 NG Flow Tag"), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()).Result)
                .Returns(() =>
                {
                    Series series = new Series();
                    series.SeriesUri = mockGasSeries;
                    series.DataPoints = new List<DataPoint>()
                        {
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 4, TimeZones.GetUtcOffset()),
                                Value = 20
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 5, TimeZones.GetUtcOffset()),
                                Value = 25
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 6, TimeZones.GetUtcOffset()),
                                Value = 30
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 7, TimeZones.GetUtcOffset()),
                                Value = 25
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 8, TimeZones.GetUtcOffset()),
                                Value = 20
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 9, TimeZones.GetUtcOffset()),
                                Value = 15
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 10, TimeZones.GetUtcOffset()),
                                Value = 25
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 11, TimeZones.GetUtcOffset()),
                                Value = 25
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 12, TimeZones.GetUtcOffset()),
                                Value = 30
                            },
                            new DataPoint
                            {
                                Series = series,
                                Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 13, TimeZones.GetUtcOffset()),
                                Value = 25
                            }
                        };
                    return series;
                });

            dataSource.Setup(x => x.GetTimeSeriesAsync(It.Is<DataSourceSeriesUri>(x => x.Uri == "B1 Steam Output Tag"), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()).Result)
                .Returns(() =>
                {
                    Series series = new Series();
                    series.SeriesUri = mockSteamSeries;
                    series.DataPoints = new List<DataPoint>()
                    {
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 4, TimeZones.GetUtcOffset()),
                            Value = 1
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 5, TimeZones.GetUtcOffset()),
                            Value = 2
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 6, TimeZones.GetUtcOffset()),
                            Value = 3
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 7, TimeZones.GetUtcOffset()),
                            Value = 2
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 8, TimeZones.GetUtcOffset()),
                            Value = 1
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 9, TimeZones.GetUtcOffset()),
                            Value = 2
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 10, TimeZones.GetUtcOffset()),
                            Value = 2.5
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 11, TimeZones.GetUtcOffset()),
                            Value = 3
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 12, TimeZones.GetUtcOffset()),
                            Value = 3.5
                        },
                        new DataPoint
                        {
                            Series = series,
                            Timestamp = new DateTimeOffset(2022, 9, 18, 6, 5, 13, TimeZones.GetUtcOffset()),
                            Value = 3
                        }
                    };
                    return series;
                });

            return dataSource;
        }
    }
}
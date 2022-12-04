using DotNetCommon.Helpers;
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

namespace Tests.EmissionsMonitorModel.TimeSeriesTests
{
    [TestClass]
    public class SeriesTests
    {
        [TestMethod]
        public void WhenSourceAndTargetResolutionAreSameAndRateMatchesResolution_RenderingIsSame()
        {
            Series series = new Series
            {
                SeriesUri = new DataSourceSeriesUri 
                { 
                    SeriesDataResolution = DataResolution.Hourly,
                    SeriesUnitRate = UnitRates.PerHour
                },
                DataPoints = new List<DataPoint>
                {
                    new DataPoint { Value = 50, Timestamp = new DateTimeOffset(2022, 11, 1, 3, 0, 0, TimeZones.GetUtcOffset()) },
                    new DataPoint { Value = 70, Timestamp = new DateTimeOffset(2022, 11, 1, 4, 0, 0, TimeZones.GetUtcOffset()) },
                    new DataPoint { Value = 20, Timestamp = new DateTimeOffset(2022, 11, 1, 5, 0, 0, TimeZones.GetUtcOffset())  },
                    new DataPoint { Value = 25, Timestamp = new DateTimeOffset(2022, 11, 1, 6, 0, 0, TimeZones.GetUtcOffset())  }
                }
            };

            QueryRenderSettings renderSettings = new QueryRenderSettings
            {
                StartDateTime = new DateTimeOffset(2022, 11, 1, 3, 0, 0, TimeZones.GetUtcOffset()),
                EndDateTime = new DateTimeOffset(2022, 11, 1, 6, 0, 0, TimeZones.GetUtcOffset()),
                RenderResolution = DataResolution.Hourly
            };
            Series normalizedSeries = series.RenderSeriesAtTargetResolution(renderSettings);

            normalizedSeries.DataPoints.First(x => x.Timestamp.Hour == 3).Value.Should().Be(50);
            normalizedSeries.DataPoints.First(x => x.Timestamp.Hour == 4).Value.Should().Be(70);
            normalizedSeries.DataPoints.First(x => x.Timestamp.Hour == 5).Value.Should().Be(20);
            normalizedSeries.DataPoints.First(x => x.Timestamp.Hour == 6).Value.Should().Be(25);
        }

        [TestMethod]
        public void WhenSourceAndTargetResolutionAreSameAndRateIsMoreFrequentThanMatchResolution_RenderingIsAdjustedByRate()
        {
            Series series = new Series
            {
                SeriesUri = new DataSourceSeriesUri 
                { 
                    SeriesDataResolution = DataResolution.Hourly,
                    SeriesUnitRate = UnitRates.PerMinute
                },
                DataPoints = new List<DataPoint>
                {
                    new DataPoint { Value = 50, Timestamp = new DateTimeOffset(2022, 11, 1, 3, 0, 0, TimeZones.GetUtcOffset())  },
                    new DataPoint { Value = 70, Timestamp = new DateTimeOffset(2022, 11, 1, 4, 0, 0, TimeZones.GetUtcOffset())  }
                }
            };
            QueryRenderSettings renderSettings = new QueryRenderSettings
            {
                StartDateTime = new DateTimeOffset(2022, 11, 1, 3, 0, 0, TimeZones.GetUtcOffset()),
                EndDateTime = new DateTimeOffset(2022, 11, 1, 4, 0, 0, TimeZones.GetUtcOffset()),
                RenderResolution = DataResolution.Hourly
            };
            Series normalizedSeries = series.RenderSeriesAtTargetResolution(renderSettings);

            normalizedSeries.DataPoints.First(x => x.Timestamp.Hour == 3).Value.Should().Be(50*60);
            normalizedSeries.DataPoints.First(x => x.Timestamp.Hour == 4).Value.Should().Be(70*60);
        }

        [TestMethod]
        public void WhenSourceAndTargetResolutionAreSameAndRateIsLessFrequentThanMatchResolution_RenderingIsAdjustedByRate()
        {
            Series series = new Series
            {
                SeriesUri = new DataSourceSeriesUri
                {
                    SeriesDataResolution = DataResolution.EveryMinute,
                    SeriesUnitRate = UnitRates.PerHour
                },
                DataPoints = new List<DataPoint>
                {
                    new DataPoint { Value = 50, Timestamp = new DateTimeOffset(2022, 11, 1, 3, 1, 0, TimeZones.GetUtcOffset())  },
                    new DataPoint { Value = 70, Timestamp = new DateTimeOffset(2022, 11, 1, 3, 2, 0, TimeZones.GetUtcOffset())  }
                }
            };
            QueryRenderSettings renderSettings = new QueryRenderSettings
            {
                StartDateTime = new DateTimeOffset(2022, 11, 1, 3, 1, 0, TimeZones.GetUtcOffset()),
                EndDateTime = new DateTimeOffset(2022, 11, 1, 3, 2, 0, TimeZones.GetUtcOffset()),
                RenderResolution = DataResolution.EveryMinute
            };
            Series normalizedSeries = series.RenderSeriesAtTargetResolution(renderSettings);

            normalizedSeries.DataPoints.First(x => x.Timestamp.Minute == 1).Value.Should().Be(50 / 60.0);
            normalizedSeries.DataPoints.First(x => x.Timestamp.Minute == 2).Value.Should().Be(70 / 60.0);
        }

        [TestMethod]
        public void WhenSourceAndTargetResolutionAreSameAndRateIsMuchLessFrequentThanMatchResolution_RenderingIsAdjustedByRate()
        {
            Series series = new Series
            {
                SeriesUri = new DataSourceSeriesUri
                {
                    SeriesDataResolution = DataResolution.EverySecond,
                    SeriesUnitRate = UnitRates.PerHour
                },
                DataPoints = new List<DataPoint>
                {
                    new DataPoint { Value = 50, Timestamp = new DateTimeOffset(2022, 11, 1, 3, 5, 1, TimeZones.GetUtcOffset())  },
                    new DataPoint { Value = 70, Timestamp = new DateTimeOffset(2022, 11, 1, 3, 5, 2, TimeZones.GetUtcOffset())  }
                }
            };
            QueryRenderSettings renderSettings = new QueryRenderSettings
            {
                StartDateTime = new DateTimeOffset(2022, 11, 1, 3, 5, 1, TimeZones.GetUtcOffset()),
                EndDateTime = new DateTimeOffset(2022, 11, 1, 3, 5, 2, TimeZones.GetUtcOffset()),
                RenderResolution = DataResolution.EverySecond
            };
            Series normalizedSeries = series.RenderSeriesAtTargetResolution(renderSettings);

            normalizedSeries.DataPoints.First(x => x.Timestamp.Second == 1).Value.Should().Be(50 / 3600.0);
            normalizedSeries.DataPoints.First(x => x.Timestamp.Second == 2).Value.Should().Be(70 / 3600.0);
        }
    }
}

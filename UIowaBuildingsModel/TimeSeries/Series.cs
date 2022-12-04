using DotNetCommon.Extensions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorModel.TimeSeries
{
    public class Series
    {
        public DataSourceSeriesUri SeriesUri { get; set; }
        public ICollection<DataPoint> DataPoints { get; set; }

        public Series RenderSeriesAtTargetResolution(QueryRenderSettings renderSettings)
        {
            Series newSeries = new Series { SeriesUri = this.SeriesUri };
            ICollection<DataPoint> renderedDataPoints = new List<DataPoint>();

            foreach (DateTimeOffset timePoint in renderSettings.BuildTimePoints())
            {
                if (SeriesUri.SeriesDataResolution.IsSameResolutionAs(renderSettings.RenderResolution))
                {
                    DataPoint matchingDataPoint = this.DataPoints.First(x => x.Timestamp == timePoint);
                    matchingDataPoint = IntegrateRateAndBuildDataPoint(newSeries, matchingDataPoint, renderSettings.RenderResolution, SeriesUri.SeriesUnitRate);
                    renderedDataPoints.Add(matchingDataPoint);
                }
                else if (SeriesUri.SeriesDataResolution.IsLessGranularThan(renderSettings.RenderResolution))
                {
                    DataPoint closestPoint = this.DataPoints.First(x => x.Timestamp == timePoint.TruncateTo(SeriesUri.SeriesDataResolution));
                    DataPoint fabricatedPoint = new DataPoint { Timestamp = timePoint, Value = ChopValue(closestPoint.Value, SeriesUri.SeriesDataResolution, renderSettings.RenderResolution) };
                    fabricatedPoint = IntegrateRateAndBuildDataPoint(newSeries, fabricatedPoint, renderSettings.RenderResolution, SeriesUri.SeriesUnitRate);
                    renderedDataPoints.Add(fabricatedPoint);
                }
                else if (SeriesUri.SeriesDataResolution.IsMoreGranularThan(renderSettings.RenderResolution))
                {
                    var involvedPoints = this.DataPoints.Where(x => x.Timestamp.TruncateTo(SeriesUri.SeriesDataResolution) == timePoint);
                    double sum = involvedPoints.Sum(x => x.Value);
                    DataPoint encapsulatingPoint = new DataPoint
                    {
                        Timestamp = timePoint,
                        Value = sum,
                        Series = newSeries
                    };
                    encapsulatingPoint = IntegrateRateAndBuildDataPoint(newSeries, encapsulatingPoint, renderSettings.RenderResolution, SeriesUri.SeriesUnitRate);
                    renderedDataPoints.Add(encapsulatingPoint);
                }
                else throw new InvalidOperationException("Unexecpected error");
            }

            newSeries.DataPoints = renderedDataPoints;
            return newSeries;
        }

        private double ChopValue(double value, string currentResolution, string desiredResolution)
        {
            if (currentResolution == DataResolution.Hourly)
            {
                if (desiredResolution == DataResolution.Hourly) return value;
                else if (desiredResolution == DataResolution.EveryMinute) return value / 60;
                else if (desiredResolution == DataResolution.EverySecond) return value / 3600;
            }
            else if (currentResolution == DataResolution.EveryMinute)
            {
                if (desiredResolution == DataResolution.EveryMinute) return value;
                else if (desiredResolution == DataResolution.EverySecond) return value / 60;
            }
            else if (currentResolution == DataResolution.EverySecond)
            {
                if (desiredResolution == DataResolution.EverySecond) return value;
            }
            throw new InvalidOperationException();
        }

        private DataPoint IntegrateRateAndBuildDataPoint(Series newSeries, DataPoint dataPoint, string renderResolution, string unitRate)
        {
            double value;
            if (unitRate == UnitRates.NoRate) value = dataPoint.Value;

            else if (renderResolution == DataResolution.Hourly)
            {
                if (unitRate == UnitRates.PerHour) value = dataPoint.Value;
                else if (unitRate == UnitRates.PerMinute) value = dataPoint.Value * 60;
                else if (unitRate == UnitRates.PerSecond) value = dataPoint.Value * 3600;
                else throw new NotImplementedException();
            }
            else if (renderResolution == DataResolution.EveryMinute)
            {
                if (unitRate == UnitRates.PerHour) value = dataPoint.Value / 60;
                else if (unitRate == UnitRates.PerMinute) value = dataPoint.Value;
                else if (unitRate == UnitRates.PerSecond) value = dataPoint.Value * 60;
                else throw new NotImplementedException();
            }
            else if (renderResolution == DataResolution.EverySecond)
            {
                if (unitRate == UnitRates.PerHour) value = dataPoint.Value / 3600;
                else if (unitRate == UnitRates.PerMinute) value = dataPoint.Value / 60;
                else if (unitRate == UnitRates.PerSecond) value = dataPoint.Value;
                else throw new NotImplementedException();
            }
            else throw new NotImplementedException();
            return new DataPoint { Timestamp = dataPoint.Timestamp, Value = value, Series = newSeries };
        }
    }
}

using DotNetCommon.Extensions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorModel.TimeSeries
{
    public class Series
    {
        public DataSourceSeriesUri SeriesUri { get; set; }
        public ICollection<DataPoint> DataPoints { get; set; }

        public Series RenderSeriesAtTargetResolution(TimeSeriesRenderSettings renderSettings)
        {
            Series newSeries = new Series { SeriesUri = this.SeriesUri };
            ICollection<DataPoint> dataPoints = new List<DataPoint>();

            foreach(DateTimeOffset timePoint in renderSettings.BuildTimePoints())
            {
                if (SeriesUri.SeriesDataResolution == renderSettings.RenderResolution)
                {
                    DataPoint dataPoint = BuildDataPoint(newSeries, this.DataPoints.First(x => x.Timestamp == timePoint), renderSettings.RenderResolution, SeriesUri.SeriesUnitRate);
                    dataPoints.Add(dataPoint);
                }
            }

            newSeries.DataPoints = dataPoints;
            return newSeries;
        }

        private DataPoint BuildDataPoint(Series newSeries, DataPoint dataPoint, string renderResolution, string unitRate)
        {
            double value;
            if (renderResolution == DataResolution.Hourly)
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

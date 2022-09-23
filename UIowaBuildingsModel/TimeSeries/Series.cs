using EmissionsMonitorModel.DataSources;

namespace EmissionsMonitorModel.TimeSeries
{
    public class Series
    {
        public DataSourceSeriesUri SeriesUri { get; set; }
        public IEnumerable<DataPoint> DataPoints { get; set; }
    }
}

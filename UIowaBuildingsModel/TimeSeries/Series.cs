namespace EmissionsMonitorModel.TimeSeries
{
    public class Series
    {
        public string SeriesName { get; set; }
        public IEnumerable<DataPoint> DataPoints { get; set; }
    }
}

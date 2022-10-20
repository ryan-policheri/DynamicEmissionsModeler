namespace EmissionsMonitorModel.DataSources
{
    public class DataSourceSeriesUri
    {
        public int DataSourceId { get; set; }
        public DataSourceBase DataSource { get; set; }

        public string SeriesName { get; set; }

        public string Prefix { get; set;  }

        public string Uri { get; set; }
    }
}
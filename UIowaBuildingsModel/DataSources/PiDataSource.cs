namespace EmissionsMonitorModel.DataSources
{
    public class PiDataSource : DataSourceBase
    {
        public PiDataSource() : base()
        {
            SourceType = DataSourceType.Pi;
        }

        public string BaseUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
namespace EmissionsMonitorModel.DataSources
{
    public class EiaDataSource : DataSourceBase
    {
        public EiaDataSource() : base()
        {
            SourceType = DataSourceType.Eia;
        }

        public string SuggestedBaseUrl { get; set; } = "https://api.eia.gov";

        public string BaseUrl { get; set; }

        public string SubscriptionKey { get; set; }
    }
}
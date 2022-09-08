using System.ComponentModel.DataAnnotations;

namespace EmissionsMonitorModel.DataSources
{
    public class EiaDataSource : DataSourceBase
    {
        public EiaDataSource() : base()
        {
            SourceType = DataSourceType.Eia;
        }

        public string SuggestedBaseUrl { get; set; } = "https://api.eia.gov";

        [Required]
        public string BaseUrl { get; set; }

        [Required] 
        public string SubscriptionKey { get; set; }
    }
}
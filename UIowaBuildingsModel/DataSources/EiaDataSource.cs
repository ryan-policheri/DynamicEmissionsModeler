using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionsMonitorModel.DataSources
{
    public class EiaDataSource : DataSourceBase
    {
        public EiaDataSource() : base()
        {
            SourceType = DataSourceType.Eia;
        }

        [NotMapped]
        public string SuggestedBaseUrl { get; set; } = "https://api.eia.gov";

        [Required]
        public string BaseUrl { get; set; }

        [Required] 
        public string SubscriptionKey { get; set; }
    }
}
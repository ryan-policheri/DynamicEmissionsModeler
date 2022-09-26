using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

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

        public override string ToSourceDetails()
        {
            dynamic obj = new ExpandoObject();
            obj.BaseUrl = this.BaseUrl;
            obj.SubscriptionKey = this.SubscriptionKey;
            return obj.ToJson();
        }
    }
}
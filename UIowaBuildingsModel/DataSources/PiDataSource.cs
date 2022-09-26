using System.ComponentModel.DataAnnotations;
using PiModel;
using System.Dynamic;

namespace EmissionsMonitorModel.DataSources
{
    public class PiDataSource : DataSourceBase, IPiConnectionInfo
    {
        public const string PI_TAG_PREFIX = "PI_TAG";
        public const string PI_AF_PREFIX = "PI_AF";

        public PiDataSource() : base()
        {
            SourceType = DataSourceType.Pi;
        }

        [Required]
        public string BaseUrl { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string DefaultAssetServer { get; set; }

        public override string ToSourceDetails()
        {
            dynamic obj = new ExpandoObject();
            obj.BaseUrl = this.BaseUrl;
            obj.UserName = this.UserName;
            obj.Password = this.Password;
            obj.DefaultAssetServer = this.DefaultAssetServer;
            return obj.ToJson();
        }
    }
}
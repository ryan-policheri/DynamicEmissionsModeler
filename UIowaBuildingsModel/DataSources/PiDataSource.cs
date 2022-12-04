using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using DotNetCommon.Extensions;
using EmissionsMonitorModel.ProcessModeling;
using PiModel;

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
            return ((object)obj).ToJson();
        }

        public static string ResolutionToPiString(string resolution)
        {
            switch (resolution)
            {
                case DataResolution.EverySecond: return "1s";
                case DataResolution.EveryMinute: return "1m";
                case DataResolution.Hourly: return "1h";
                case DataResolution.Daily: return "1d";
                default: throw new NotImplementedException($"need to implement resolution {resolution}");
            }
        }
    }
}
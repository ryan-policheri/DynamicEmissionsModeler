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

        public string BaseUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DefaultAssetServer { get; set; }
    }
}
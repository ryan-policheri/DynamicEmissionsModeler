using DotNetCommon.WebApiClient;
using EmissionsMonitorDataAccess.Http;

namespace EmissiosMonitorDataAccess.Http
{
    public class EmissionsMonitorClient : WebApiClientBase
    {
        public EmissionsMonitorClient()
        {
            this.Client = new HttpClient();
            this.SerializerOptions = JsonSerializerDefaults.CamelCaseOptions;
        }

        public void Initialize(IEmissionsMonitorClientConfig config)
        {
            this.Client.BaseAddress = new Uri(config.EmissionsMonitorApiBaseUrl);
        }
    }
}
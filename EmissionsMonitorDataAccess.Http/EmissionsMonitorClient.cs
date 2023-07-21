using DotNetCommon.WebApiClient;
using EmissionsMonitorDataAccess.Http;

namespace EmissiosMonitorDataAccess.Http
{
    public class EmissionsMonitorClient : WebApiClientBase
    {
        public EmissionsMonitorClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            //TODO: Figure certificate out
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            this.Client = new HttpClient(handler);
            this.SerializerOptions = JsonSerializerDefaults.CamelCaseOptions;
        }

        public void Initialize(IEmissionsMonitorClientConfig config)
        {
            this.Client.BaseAddress = new Uri(config.EmissionsMonitorApiBaseUrl);
        }
    }
}
using DotNetCommon.WebApiClient;
using System.Net;

namespace PiServices
{
    public class PiHttpClient : WebApiClientBase
    {
        public PiHttpClient(string baseAddress, string basicAuthString) : base()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            this.Client = new HttpClient(handler);
            this.Client.DefaultRequestHeaders.Add("Authorization", basicAuthString);
            this.Client.BaseAddress = new Uri(baseAddress);
        }

        public async Task DoSomething()
        {
            string data = await this.GetAsync("assetservers");
        }
    }
}
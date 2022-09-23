namespace EIA.Services.Clients
{
    public interface IEiaConnectionInfo
    {
        public string BaseUrl { get; set; }
        public string SubscriptionKey { get; set; }
    }
}
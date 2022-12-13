namespace PiModel
{
    public interface IPiConnectionInfo
    {
        public string BaseUrl { get; }

        public string UserName { get; }

        public string Password { get; }

        public string DefaultAssetServer { get; }
    }
}

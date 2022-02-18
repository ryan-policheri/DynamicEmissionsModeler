namespace PiModel
{
    public class AssetServer : ItemBase
    {
        public bool IsConnected { get; set; }
        public string ServerVersion { get; set; }
        public AssetServerLinks Links { get; set; }
    }
}

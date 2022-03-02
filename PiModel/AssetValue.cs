namespace PiModel
{
    public class AssetValue
    {
        public string WebId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public AssetValueLinks Links { get; set; }
        public Value Value { get; set; }
    }
}

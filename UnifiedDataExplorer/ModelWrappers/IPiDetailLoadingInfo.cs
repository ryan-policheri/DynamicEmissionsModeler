namespace UnifiedDataExplorer.ModelWrappers
{
    public interface IPiDetailLoadingInfo
    {
        public int DataSourceId { get; set; }

        public string Id { get; set; }

        public string Verb { get; set; }

        public string TypeTag { get; set; }
    }
}

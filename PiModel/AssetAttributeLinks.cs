namespace PiModel
{
    public class AssetAttributeLinks : IHaveTimeSeriesDataLinks
    {
        public string Self { get; set; }
        public string Attributes { get; set; }
        public string Element { get; set; }
        public string Template { get; set; }
        public string InterpolatedData { get; set; }
        public string RecordedData { get; set; }
        public string PlotData { get; set; }
        public string SummaryData { get; set; }
        public string Value { get; set; }
        public string EndValue { get; set; }
        public string Categories { get; set; }
    }
}

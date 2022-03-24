namespace PiModel
{
    public class PiPointLinks : IHaveTimeSeriesDataLinks
    {
        public string Self { get; set; }
        public string DataServer { get; set; }
        public string Attributes { get; set; }
        public string InterpolatedData { get; set; }
        public string RecordedData { get; set; }
        public string PlotData { get; set; }
        public string SummaryData { get; set; }
        public string Value { get; set; }
        public string EndValue { get; set; }
    }
}
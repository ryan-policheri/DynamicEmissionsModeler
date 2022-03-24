namespace PiModel
{
    public interface IHaveTimeSeriesDataLinks
    {
        public string InterpolatedData { get; }
        public string RecordedData { get; }
        public string PlotData { get; }
        public string SummaryData { get; }
    }
}
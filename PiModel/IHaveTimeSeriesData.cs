namespace PiModel
{
    public interface IHaveTimeSeriesData
    {
        public IEnumerable<InterpolatedDataPoint> InterpolatedDataPoints { get; set; }

        public IHaveTimeSeriesDataLinks TimeSeriesLinks { get; }
    }
}
using System.Data;

namespace PiModel
{
    public interface IHaveTimeSeriesData
    {
        public IEnumerable<InterpolatedDataPoint> InterpolatedDataPoints { get; set; }

        public IEnumerable<SummaryResult> SummaryDataPoints { get; set; }

        public IHaveTimeSeriesDataLinks TimeSeriesLinks { get; }
    }
}
using System.Text.Json.Serialization;

namespace PiModel
{
    public class PiPoint : IHaveTimeSeriesData
    {
        public string WebId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Descriptor { get; set; }
        public string PointClass { get; set; }
        public string PointType { get; set; }
        public string DigitalSetName { get; set; }
        public string EngineeringUnits { get; set; }
        public double Span { get; set; }
        public double Zero { get; set; }
        public bool Step { get; set; }
        public bool Future { get; set; }
        public int DisplayDigits { get; set; }
        public PiPointLinks Links { get; set; }

        [JsonIgnore]
        IHaveTimeSeriesDataLinks IHaveTimeSeriesData.TimeSeriesLinks => Links;

        public IEnumerable<InterpolatedDataPoint> InterpolatedDataPoints { get; set; }
    }
}
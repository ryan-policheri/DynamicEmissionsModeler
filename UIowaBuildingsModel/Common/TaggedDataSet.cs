using PiModel;

namespace UIowaBuildingsModel
{
    public class TaggedDataSet
    {
        public string Tag { get; set; }
        public string FilterExpression { get; set; }

        public string Description { get; set; }
        public string Units { get; set; }
        public IEnumerable<InterpolatedDataPoint> DataPoints { get; set; }

        public void PopulateData(PiPoint piPoint)
        {
            DataPoints = piPoint.InterpolatedDataPoints;
            Description = piPoint.Descriptor;
            Units = piPoint.EngineeringUnits;
        }
    }
}
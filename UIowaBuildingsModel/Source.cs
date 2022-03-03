namespace UIowaBuildingsModel
{
    public class Source
    {
        public string SourceName { get; set; }
        public string HourlySourceId { get; set; }
        public double KiloGramsOfCo2PerKwh { get; set; }

        public HourlySource ProduceHour(DateTime hour, double value)
        {
            return new HourlySource
            {
                Source = this,
                Hour = hour,
                Value = value
            };
        }
    }
}
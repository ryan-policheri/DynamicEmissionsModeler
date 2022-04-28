namespace UIowaBuildingsModel
{
    public class HourlyEmissionsReportParameters
    {
        public HourlyEmissionsReportParameters()
        {
            AssetLinks = new List<string>();
        }

        private DateTime _startDate;
        public DateTime StartDateInLocalTime 
        { 
            get { return _startDate; }
            set
            {
                if (value.Kind != DateTimeKind.Local) throw new ArgumentException("Expected date to be in local time");
                _startDate = value;
            }
        }

        private DateTime _endDate;
        public DateTime EndDateInLocalTime
        {
            get { return _endDate; }
            set
            {
                if (value.Kind != DateTimeKind.Local) throw new ArgumentException("Expected date to be in local time");
                _endDate = value;
            }
        }

        public ICollection<string> AssetLinks { get; set; }
    }

    public enum ElectricGridStrategy
    {
        MisoHourly,
        MidAmericanAverageFuelMix
    }
}
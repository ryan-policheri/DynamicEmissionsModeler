namespace UIowaBuildingsModel
{
    public class HourlyEmissionsReportParameters
    {
        public HourlyEmissionsReportParameters()
        {
            AssetLinks = new List<string>();
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<string> AssetLinks { get; set; }
    }
}
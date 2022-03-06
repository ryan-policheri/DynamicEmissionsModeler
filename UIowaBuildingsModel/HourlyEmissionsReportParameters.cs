namespace UIowaBuildingsModel
{
    public class HourlyEmissionsReportParameters
    {
        public HourlyEmissionsReportParameters()
        {
            AssetLinks = new List<string>();
        }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public ICollection<string> AssetLinks { get; set; }
    }
}
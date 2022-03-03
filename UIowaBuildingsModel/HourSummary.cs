namespace UIowaBuildingsModel
{
    public class HourSummary
    {
        public DateTime Hour { get; set; }

        public ICollection<HourlySource> Sources { get; } = new List<HourlySource>();

        private double TotalGeneratedInMegawattHours => Sources.Sum(x => x.Value);

        public double CalculateHourlyCO2EmissionsInKg(double kilowattHours)
        {
            double total = 0;
            foreach(HourlySource source in Sources)
            {
                total += CalculateCO2EmissionsInKgFromSource(source, kilowattHours);
            }
            return total;
        }

        public double CalculateCO2EmissionsInKgFromSource(HourlySource hourlySource, double kilowattHours)
        {
            Source source = hourlySource.Source;
            if (source.KiloGramsOfCo2PerKwh <= 0) return 0;
            else return (CalculateSourcePercentage(hourlySource) * kilowattHours) * source.KiloGramsOfCo2PerKwh; 
        }

        private double CalculateSourcePercentage(HourlySource source)
        {
            return source.Value / TotalGeneratedInMegawattHours;
        }
    }
}

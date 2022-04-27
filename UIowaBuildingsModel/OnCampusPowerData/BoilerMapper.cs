using DotNetCommon.Extensions;
using PiModel;
using UnitsNet;

namespace UIowaBuildingsModel
{
    public class BoilerMapper
    {
        public string BoilerName { get; set; }
        public IEnumerable<BoilerInputMapper> BoilerInputMappers { get; set; }
       
        public string SteamOutputTag { get; set; }
        public IEnumerable<InterpolatedDataPoint> OutputDataPoints { get; set; }
        public Func<InterpolatedDataPoint, Energy> OuputToEnergyContentFunction { get; set; }

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            IEnumerable<DateTimeOffset> outputDataHours = OutputDataPoints.Select(x => x.Timestamp);

            foreach (BoilerInputMapper inputMapper in BoilerInputMappers) //Verify that all the timestamps line up for each data set
            {
                bool allHoursMatch = outputDataHours.AllHoursMatch(inputMapper.InputDataPoints.Select(x => x.Timestamp));
                if (!allHoursMatch) throw new InvalidDataException("The timestamps between the associated data sets do not match");
            }

            return outputDataHours.ToList().Copy();
        }

        public IEnumerable<SteamSource> PackageDataIntoSteamSources()
        {
            ICollection<SteamSource> steamSources = new List<SteamSource>();

            foreach(DateTimeOffset hour in this.EnumerateDataHours())
            {
                Mass co2 = Mass.Zero;

                foreach(BoilerInputMapper inputMapper in this.BoilerInputMappers)
                {
                    InterpolatedDataPoint inputDataPoint = inputMapper.InputDataPoints.FindMatchingHour(hour);
                    co2 += inputMapper.DataPointToCo2EmissionsFunction(inputDataPoint);
                }

                InterpolatedDataPoint outputDataPoint = this.OutputDataPoints.FindMatchingHour(hour);

                SteamSource steamSource = new SteamSource
                {
                    Timestamp = hour,
                    SourceName = this.BoilerName,
                    InputNames = this.BoilerInputMappers.OrderBy(x => x.InputName).Select(x => x.InputName),
                    Co2FromInputs = co2,
                    SteamEnergyContent = this.OuputToEnergyContentFunction(outputDataPoint)
                };

                steamSources.Add(steamSource);
            }

            return steamSources;
        }
    }
}
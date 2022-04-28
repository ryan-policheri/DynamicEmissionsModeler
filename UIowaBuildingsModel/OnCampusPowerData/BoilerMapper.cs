using System.Data;
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

        public DataTable BuildDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Hour");

            foreach (BoilerInputMapper mapper in this.BoilerInputMappers) { table.Columns.Add(mapper.InputTag); }
            table.Columns.Add("Input Total Co2 (kg)");
            table.Columns.Add("Steam Output (pi)");
            table.Columns.Add("Steam Energy Content (mmbtu)");

            var steamSources = this.PackageDataIntoSteamSources();

            foreach (DateTimeOffset hour in this.EnumerateDataHours())
            {
                DataRow row = table.NewRow();
                row["Hour"] = hour.LocalDateTime.ToString();

                foreach (BoilerInputMapper mapper in this.BoilerInputMappers) 
                { 
                    InterpolatedDataPoint inputPoint = mapper.InputDataPoints.FindMatchingHour(hour);
                    row[mapper.InputTag] = inputPoint.Value;
                }

                SteamSource match = steamSources.First(x => x.Timestamp.HourMatches(hour));
                row["Input Total Co2 (kg)"] = match.Co2FromInputs.Kilograms;

                InterpolatedDataPoint outputPoint = OutputDataPoints.FindMatchingHour(hour);
                row["Steam Output (pi)"] = outputPoint.Value;

                row["Steam Energy Content (mmbtu)"] = match.SteamEnergyContent.MegabritishThermalUnits;

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
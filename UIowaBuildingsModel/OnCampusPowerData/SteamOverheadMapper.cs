using DotNetCommon.Extensions;
using PiModel;
using UnitsNet;

namespace EmissionsMonitorModel
{
    public class SteamOverheadMapper
    {
        public List<SteamOverheadFactorMapper> FactorMappers { get; set; }

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            IEnumerable<DateTimeOffset> outputDataHours = this.FactorMappers.First().DataPoints.Select(x => x.Timestamp);

            foreach (SteamOverheadFactorMapper factorMapper in this.FactorMappers)
            {
                bool allHoursMatch = outputDataHours.AllHoursMatch(factorMapper.DataPoints.Select(x => x.Timestamp));
                if (!allHoursMatch) throw new InvalidDataException("The timestamps between the associated data sets do not match");
            }

            return outputDataHours.ToList().Copy();
        }

        public IEnumerable<SteamOverhead> PackageIntoSteamOverheads()
        {
            ICollection<SteamOverhead> result = new List<SteamOverhead>();

            foreach (DateTimeOffset hour in this.EnumerateDataHours())
            {
                SteamOverhead steamOverhead = new SteamOverhead();
                Energy hourlySteamOverhead = Energy.Zero;

                foreach (SteamOverheadFactorMapper factorMapper in this.FactorMappers)
                {
                    InterpolatedDataPoint dataPoint = factorMapper.DataPoints.FindMatchingHour(hour);
                    Energy steamEnergy = factorMapper.DataPointToSteamEnergyFunction(dataPoint);
                    hourlySteamOverhead += steamEnergy;
                }

                steamOverhead.Timestamp = hour;
                steamOverhead.SteamEnergy = hourlySteamOverhead;
                result.Add(steamOverhead);
            }

            return result;
        }
    }
}
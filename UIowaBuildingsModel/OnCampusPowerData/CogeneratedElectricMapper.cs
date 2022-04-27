using DotNetCommon.Extensions;
using UnitsNet;

namespace UIowaBuildingsModel
{
    public class CogeneratedElectricMapper
    {
        public IEnumerable<SteamToElectricTurbineMapper> TurbineProcessMappers { get; set; }

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            IEnumerable<DateTimeOffset> dataHours = this.TurbineProcessMappers.First().InvolvedDataSets.First().DataPoints.Select(x => x.Timestamp);

            foreach (SteamToElectricTurbineMapper turbineMapper in this.TurbineProcessMappers)
            {
                foreach(TaggedDataSet dataSet in turbineMapper.InvolvedDataSets)
                {
                    bool allHoursMatch = dataHours.AllHoursMatch(dataSet.DataPoints.Select(x => x.Timestamp));
                    if (!allHoursMatch) throw new InvalidDataException("The timestamps between the associated data sets do not match");
                }
            }

            return dataHours.ToList().Copy();
        }

        public IEnumerable<CogeneratedElectric> PackageDataIntoCoGeneratedElectric()
        {
            IEnumerable<DateTimeOffset> dataHours = EnumerateDataHours();
            ICollection<CogeneratedElectric> result = new List<CogeneratedElectric>();

            foreach(DateTimeOffset dataHour in dataHours)
            {
                Energy totalSteamEnergyUsed = Energy.Zero;
                
                foreach(SteamToElectricTurbineMapper turbineMapper in TurbineProcessMappers)
                {
                    IEnumerable<TaggedDataPoint> dataPoints = turbineMapper.InvolvedDataSets.Select(x => new TaggedDataPoint { Tag = x.Tag, DataPoint = x.DataPoints.Where(y => y.Timestamp == dataHour).First() });
                    Energy turbineSteamEnergyUsed = turbineMapper.TurbineProcessDataToEnergyFunction(dataPoints);
                    totalSteamEnergyUsed += turbineSteamEnergyUsed;
                }

                result.Add(new CogeneratedElectric
                {
                    SteamTurbineNames = this.TurbineProcessMappers.Select(x => x.TurbineName),
                    Timestamp = dataHour,
                    SteamEnergyUsed = totalSteamEnergyUsed
                });
            }

            return result;
        }
    }
}
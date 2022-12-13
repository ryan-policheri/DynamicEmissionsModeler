using DotNetCommon.Extensions;
using UnitsNet;
using EIA.Domain.Model;

namespace EmissionsMonitorModel
{
    public class ElectricGridSourceMapper
    {
        public string SourceName { get; set; }
        public string HourlySourceId { get; set; }
        public double KiloGramsOfCo2PerKwh { get; set; }
        public Func<SeriesDataPoint, Energy> DataPointToEnergyFunction { get; set; }
        public Func<Energy, Mass> EnergyToCo2EmissionsFunction { get; set; }
        public IEnumerable<SeriesDataPoint> DataPoints { get; set; }

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            return DataPoints.Select(x => x.Timestamp).ToList().Copy();
        }

        public IEnumerable<ElectricGridSource> PackageDataIntoElectricGridSources()
        {
            ICollection<ElectricGridSource> gridSources = new List<ElectricGridSource>();

            foreach(DateTimeOffset timestamp in EnumerateDataHours())
            {
                SeriesDataPoint dataPoint = DataPoints.FindMatchingHour(timestamp);
                Energy energyFromSource = this.DataPointToEnergyFunction(dataPoint);

                gridSources.Add(new ElectricGridSource
                {
                    Timestamp = timestamp,
                    SourceName = this.SourceName,
                    SourceId = this.HourlySourceId,
                    ElectricEnergyFromSource = energyFromSource,
                    Co2FromSource = this.EnergyToCo2EmissionsFunction(energyFromSource)
                });
            }

            return gridSources;
        }
    }
}
using DotNetCommon.Extensions;
using UnitsNet;
using PiModel;

namespace EmissionsMonitorModel
{
    public class BuildingUsageMapper
    {
        public string BuildingName { get; set; }
        public double SquareFeet { get; set; }
        public Func<InterpolatedDataPoint, Energy> DataPointToElectricUsageFunction { get; set; }
        public IEnumerable<InterpolatedDataPoint> ElectricUsageDataPoints { get; set; }

        public Func<InterpolatedDataPoint, Mass> DataPointToSteamUsageAsMassFunction { get; set; }
        public Func<InterpolatedDataPoint, Energy> DataPointToSteamUsageAsEnergyFunction { get; set; }
        public IEnumerable<InterpolatedDataPoint> SteamUsageDataPoints { get; set; }

        public Func<InterpolatedDataPoint, Volume> DataPointToChilledWaterVolumeFunction { get; set; }
        public IEnumerable<InterpolatedDataPoint> ChilledWaterUsageDataPoints { get; set; }

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            return ElectricUsageDataPoints.Select(x => x.Timestamp).ToList().Copy();
        }

        public BuildingUsageSummary PackageDataIntoBuildingUsageSummary()
        {
            BuildingUsageSummary summary = new BuildingUsageSummary();
            summary.BuildingName = BuildingName;
            summary.SquareFeet = this.SquareFeet;

            ICollection<BuildingUsage> buildingUsages = new List<BuildingUsage>();

            foreach(DateTimeOffset timestamp in EnumerateDataHours())
            {
                InterpolatedDataPoint electricDataPoint = ElectricUsageDataPoints.FindMatchingHour(timestamp);

                BuildingUsage usage = new BuildingUsage
                {
                    Timestamp = timestamp,
                    ElectricUsage = this.DataPointToElectricUsageFunction(electricDataPoint)
                };

                if(this.SteamUsageDataPoints != null) //Not all buildings use steam
                {
                    InterpolatedDataPoint steamDataPoint = SteamUsageDataPoints.FindMatchingHour(timestamp);
                    usage.SteamUsageAsMass = this.DataPointToSteamUsageAsMassFunction(steamDataPoint);
                    usage.SteamUsageAsEnergy = this.DataPointToSteamUsageAsEnergyFunction(steamDataPoint);
                }
                else
                {
                    usage.SteamUsageAsMass = Mass.Zero;
                    usage.SteamUsageAsEnergy = Energy.Zero;
                }

                if (this.ChilledWaterUsageDataPoints != null) //Not all buildings use chilled water
                {
                    InterpolatedDataPoint chilledWaterDataPoint = ChilledWaterUsageDataPoints.FindMatchingHour(timestamp);
                    usage.ChilledWaterUsage = this.DataPointToChilledWaterVolumeFunction(chilledWaterDataPoint);
                }
                else
                {
                    usage.ChilledWaterUsage = Volume.Zero;
                }

                buildingUsages.Add(usage);
            }

            summary.BuildingUsages = buildingUsages;
            return summary;
        }
    }
}
using DotNetCommon.Extensions;
using PiModel;

namespace UIowaBuildingsModel
{
    public class CampusEnergyResourceManager
    {
        public CampusEnergyResourceManager(DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            if (startDateTime == default(DateTimeOffset) || endDateTime == default(DateTimeOffset)) throw new ArgumentNullException("Start date or end date is not specified");
            if (startDateTime > endDateTime) throw new ArgumentException("Start date must be less than end date");

            StartDateTime = startDateTime;
            EndDateTime = endDateTime;

            _hoursDetails = new List<HourDetails>();
            foreach(DateTimeOffset dateTimeOffset in this.EnumerateHours()) { _hoursDetails.Add(new HourDetails(dateTimeOffset)); }
        }

        public DateTimeOffset StartDateTime { get; }
        public DateTimeOffset EndDateTime { get; }
        public int TotalHours => ((int)(this.EndDateTime - this.StartDateTime).TotalHours) + 1;      
        public IEnumerable<DateTimeOffset> EnumerateHours() => StartDateTime.EnumerateHoursUntil(EndDateTime);


        private ICollection<HourDetails> _hoursDetails;
        public IEnumerable<HourDetails> EnumerateHoursDetails() => _hoursDetails.Copy();
        public HourDetails GetHour(DateTimeOffset dateTimeOffset) => _hoursDetails.FindMatchingHour(dateTimeOffset).Copy();


        public void AddCampusElectricUsage(DataPointsToEnergyMapper electricUsageMapper)
        {
            foreach (InterpolatedDataPoint point in electricUsageMapper.DataPoints)
            {
                HourDetails hourDetails = _hoursDetails.FindMatchingHour(point.Timestamp);
                hourDetails.AddCampusElectricUsage(electricUsageMapper.DataPointToEnergyFunction(point));
            }
        }

        public void AddCampusElectricGeneration(DataPointsToEnergyMapper electricGenerationMapper)
        {
            foreach (InterpolatedDataPoint point in electricGenerationMapper.DataPoints)
            {
                HourDetails hourDetails = _hoursDetails.FindMatchingHour(point.Timestamp);
                hourDetails.AddCampusElectricGeneration(electricGenerationMapper.DataPointToEnergyFunction(point));
            }
        }

        public void AddGridOperatorElectricSource(ElectricGridSourceMapper gridSource)
        {
            IEnumerable<DateTimeOffset> dataTimeStamps = gridSource.EnumerateDataHours();
            bool matchesExpectedTimeStamps = this.EnumerateHours().AllHoursMatch(dataTimeStamps);
            if (!matchesExpectedTimeStamps) throw new InvalidDataException("Actual data timestamps do not exactly match expected timestamps");

            foreach (ElectricGridSource electricGridSource in gridSource.PackageDataIntoElectricGridSources())
            {
                HourDetails hourDetails = _hoursDetails.FindMatchingHour(electricGridSource.Timestamp);
                hourDetails.AddElectricGridSource(electricGridSource);
            }
        }

        public void AddPowerPlantData(PowerPlantDataMapper mapper)
        {
            IEnumerable<DateTimeOffset> dataTimeStamps = mapper.EnumerateDataHours();
            bool matchesExpectedTimeStamps = this.EnumerateHours().AllHoursMatch(dataTimeStamps);
            if (!matchesExpectedTimeStamps) throw new InvalidDataException("Actual data timestamps do not exactly match expected timestamps");

            AddSteamProduction(mapper.SteamProductionMapper);
            AddElectricCogeneration(mapper.CogeneratedElectricMapper);
            AddSteamOverhead(mapper.SteamOverheadMapper);
            AddElectricOverhead(mapper.ElectricOverheadMapper);
            AddChilledWaterGeneration(mapper.ChilledWaterMapper);
        }

        private void AddSteamProduction(SteamProductionMapper mapper)
        {
            foreach(BoilerMapper boilerMapper in mapper.BoilerMappers)
            {
                foreach (SteamSource steamSource in boilerMapper.PackageDataIntoSteamSources())
                {
                    HourDetails hourDetails = _hoursDetails.FindMatchingHour(steamSource.Timestamp);
                    hourDetails.AddCampusSteamSource(steamSource);
                }
            }
        }

        public void AddElectricCogeneration(CogeneratedElectricMapper mapper)
        {
            foreach (CogeneratedElectric coGeneratedElectric in mapper.PackageDataIntoCoGeneratedElectric())
            {
                HourDetails hourDetails = _hoursDetails.FindMatchingHour(coGeneratedElectric.Timestamp);
                hourDetails.AddCogeneratedElectric(coGeneratedElectric);
            }
        }

        private void AddSteamOverhead(SteamOverheadMapper mapper)
        {
            foreach(SteamOverhead steamOverhead in mapper.PackageIntoSteamOverheads())
            {
                HourDetails hourDetails = _hoursDetails.FindMatchingHour(steamOverhead.Timestamp);
                hourDetails.AddSteamOverhead(steamOverhead);
            }
        }

        private void AddElectricOverhead(ElectricOverheadMapper mapper)
        {
            foreach(ElectricOverhead electricOverhead in mapper.PackageDataIntoElectricOverheads())
            {
                HourDetails hourDetails = _hoursDetails.FindMatchingHour(electricOverhead.Timestamp);
                hourDetails.AddElectricOverhead(electricOverhead);
            }
        }

        private void AddChilledWaterGeneration(ChilledWaterProductionMapper mapper)
        {
            foreach(ChilledWaterProduced chilledWaterProduced in mapper.PackageIntoChilledWaterProduced(this.StartDateTime, this.EndDateTime))
            {
                HourDetails hourDetails = _hoursDetails.FindMatchingHour(chilledWaterProduced.Timestamp);
                hourDetails.AddChilledWaterGeneration(chilledWaterProduced);
            }
        }

        public void PopulateCo2EmissionsData(List<BuildingUsageSummary> usageSummaries)
        {
            foreach(BuildingUsageSummary summary in usageSummaries)
            {
                foreach(BuildingUsage usage in summary.BuildingUsages)
                {
                    HourDetails hourDetails = _hoursDetails.FindMatchingHour(usage.Timestamp);
                    hourDetails.PopulateCo2Emissions(usage);
                }
            }
        }
    }
}
using DotNetCommon.Extensions;
using UnitsNet;

namespace EmissionsMonitorModel
{
    public class HourDetails
    {
        private List<ElectricGridSource> _electricGridSources;

        private List<SteamSource> _campusSteamSources;
        private CogeneratedElectric _cogeneratedElectric;

        private SteamOverhead _steamOverhead;
        private ElectricOverhead _electricOverhead;

        private ChilledWaterProduced _chilledWaterProduced;

        public HourDetails(DateTimeOffset dateTimeOffset)
        {
            _electricGridSources = new List<ElectricGridSource>();
            _campusSteamSources = new List<SteamSource>();
            Hour = dateTimeOffset;
        }

        public DateTimeOffset Hour { get; }

        private Energy _campusElectricUsage = Energy.Zero;
        public Energy CampusElectricUsage 
        {
            get
            {
                //if (_campusElectricUsage == Energy.Zero) throw new InvalidOperationException("Campus electric usage has not been added for this hour");
                return _campusElectricUsage;
            }
            private set
            {
                if (_campusElectricUsage == Energy.Zero) _campusElectricUsage = value;
                else throw new InvalidOperationException("Campus electric usage has already been added");
            }
        }


        private Energy _campusElectricGeneration = Energy.Zero;
        public Energy CampusElectricGeneration
        {
            get
            {
                //if (_campusElectricGeneration == Energy.Zero) throw new InvalidOperationException("Campus electric generation has not been added for this hour");
                return _campusElectricGeneration;
            }
            private set 
            {
                if (_campusElectricGeneration == Energy.Zero) _campusElectricGeneration = value;
                else throw new InvalidOperationException("Campus electric generation has already been added");
            }
        }

        public Energy ElectricPurchased => CampusElectricUsage - CampusElectricGeneration; //TODO: get value from tag

        public double FractionElectricGeneratedOnCampus => CampusElectricGeneration / CampusElectricUsage;
        public double FractionElectricPurchasedFromGrid => 1 - FractionElectricGeneratedOnCampus;
        public double PercentElectricGeneratedOnCampus => FractionElectricGeneratedOnCampus * 100;
        public double PercentElectricPurchasedFromGrid => FractionElectricPurchasedFromGrid * 100;

        public Energy CogeneratedElectric => CampusElectricGeneration; //TODO: This is a bit of a lie. CampusElectric Generation includes non-cogenerated electic from generator and solar panels.
        public Energy SteamProduced => Energy.FromMegabritishThermalUnits(_campusSteamSources.Sum(x => x.SteamEnergyContent.MegabritishThermalUnits));
        public Volume ChilledWaterProduced => _chilledWaterProduced.ChilledWaterVolume;
        public Energy PowerPlantElectricOverhead => _electricOverhead.ElectricEnergy;
        public Energy PowerPlantSteamOverhead => _steamOverhead.SteamEnergy;

        public double ElectricEmissionsFactorInKilogramsOfCo2PerMegawattHour => CalculateKilogramsOfCo2EmissionsPerMwhOfElectric();
        public double SteamEmissionsFactorInKilogramsOfCo2PerMmbtu => CalculateKilogramsOfCo2PerMmbtuOfSteam();
        public double ChilledWaterEmissionsFactorInKilogramsOfCo2PerChilledWaterGallon => CalculateKilogramsOfCo2EmissionsPerGallonOfChilledWater();

        public IEnumerable<ElectricGridSource> EnumerateElectricGridSources() => _electricGridSources.Copy();

        public void AddElectricGridSource(ElectricGridSource electricGridSource)
        {
            if (_electricGridSources.Any(x => x.SourceId.CapsAndTrim() == electricGridSource.SourceId.CapsAndTrim()))
                throw new InvalidOperationException("Data for this source and hour has already been added");
            _electricGridSources.Add(electricGridSource);
        }

        public void AddCampusElectricUsage(Energy campusElectricUsage)
        {
            CampusElectricUsage = campusElectricUsage;
        }

        public void AddCampusElectricGeneration(Energy campusElectricGeneration)
        {
            CampusElectricGeneration = campusElectricGeneration;
        }

        public void AddCampusSteamSource(SteamSource steamSource)
        {
            _campusSteamSources.Add(steamSource);
        }

        public void AddCogeneratedElectric(CogeneratedElectric cogeneratedElectric)
        {
            if (_cogeneratedElectric != null) throw new InvalidOperationException("Steam used for electric has already been added");
            if (!cogeneratedElectric.Timestamp.HourMatches(this.Hour)) throw new ArgumentOutOfRangeException("Steam for electric data not for the right hour");
            _cogeneratedElectric = cogeneratedElectric;
        }

        public void AddSteamOverhead(SteamOverhead steamOverhead)
        {
            if (_steamOverhead != null) throw new InvalidOperationException("Boilers electric overhead has already been added");
            if (!steamOverhead.Timestamp.HourMatches(this.Hour)) throw new ArgumentOutOfRangeException("Overhead data not for the right hour");
            _steamOverhead = steamOverhead;
        }

        public void AddElectricOverhead(ElectricOverhead electricOverhead)
        {
            if (_electricOverhead != null) throw new InvalidOperationException("Boilers electric overhead has already been added");
            if (!electricOverhead.Timestamp.HourMatches(this.Hour)) throw new ArgumentOutOfRangeException("Overhead data not for the right hour");
            _electricOverhead = electricOverhead;
        }

        public void AddChilledWaterGeneration(ChilledWaterProduced chilledWaterProduced)
        {
            if (_chilledWaterProduced != null) throw new InvalidOperationException("Chilled water produced has already been added");
            if (!chilledWaterProduced.Timestamp.HourMatches(this.Hour)) throw new ArgumentOutOfRangeException("Chilled water data not for the right hour");
            _chilledWaterProduced = chilledWaterProduced;
        }

        public void PopulateCo2Emissions(BuildingUsage buildingUsage)
        {
            buildingUsage.Co2FromElectricUsage = CalculateCo2EmissionsFromElectricUsage(buildingUsage.ElectricUsage);
            buildingUsage.Co2FromSteamUsage = CalculateCo2FromSteamUsage(buildingUsage.SteamUsageAsEnergy);
            buildingUsage.Co2FromChilledWaterUsage = CalculateCo2EmissionsFromChilledWaterUsage(buildingUsage.ChilledWaterUsage);
        }

        public Mass CalculateCo2EmissionsFromElectricUsage(Energy electricUsage)
        {
            return Mass.FromKilograms(electricUsage.MegawattHours * CalculateKilogramsOfCo2EmissionsPerMwhOfElectric());
        }

        public Mass CalculateCo2FromSteamUsage(Energy buildingSteamUsage)
        {
            return Mass.FromKilograms(buildingSteamUsage.MegabritishThermalUnits * CalculateKilogramsOfCo2PerMmbtuOfSteam());
        }

        public Mass CalculateCo2EmissionsFromChilledWaterUsage(Volume chilledWaterUsage)
        {
            return Mass.FromKilograms(chilledWaterUsage.UsGallons * CalculateKilogramsOfCo2EmissionsPerGallonOfChilledWater());
        }

        private double CalculateKilogramsOfCo2PerMmbtuOfSteam()
        {
            Mass carbonInput = Mass.FromKilograms(0);
            Energy steamOutput = Energy.FromMegabritishThermalUnits(0);
            foreach (SteamSource source in _campusSteamSources)
            {
                carbonInput += source.Co2FromInputs;
                steamOutput += source.SteamEnergyContent;
            }

            Energy steamEnergyUsedForElectricOverhead = Energy.FromMegabritishThermalUnits(_electricOverhead.ElectricEnergy.MegawattHours * FractionElectricGeneratedOnCampus * CalculateMmbtusPerMwhOfCogeneratedElectric());
            steamOutput -= _steamOverhead.SteamEnergy;
            steamOutput -= steamEnergyUsedForElectricOverhead;
            
            Mass gridEmissionsFromElectricOverhead = Mass.FromKilograms(_electricOverhead.ElectricEnergy.MegawattHours * FractionElectricPurchasedFromGrid * CalculateKilogramsOfCo2EmissionsPerMwhOfGridElectric());
            carbonInput += gridEmissionsFromElectricOverhead;

            double value = carbonInput.Kilograms / steamOutput.MegabritishThermalUnits;
            return value;
        }

        private double CalculateMmbtusPerMwhOfCogeneratedElectric()
        {
            if (_campusElectricGeneration == Energy.Zero) return 0;
            double value = _cogeneratedElectric.SteamEnergyUsed.MegabritishThermalUnits / _campusElectricGeneration.MegawattHours;
            return value;
        }

        private double CalculateKilogramsOfCo2EmissionsPerMwhOfGridElectric()
        {
            Energy gridTotalGenerated = Energy.Zero;
            Mass gridTotalEmissions = Mass.Zero;

            foreach (ElectricGridSource source in _electricGridSources)
            {
                gridTotalGenerated += source.ElectricEnergyFromSource;
                gridTotalEmissions += source.Co2FromSource;
            }

            double kiloGramsOfCo2PerKwhHourOfElectricGenerated = gridTotalEmissions.Kilograms / gridTotalGenerated.MegawattHours;
            return kiloGramsOfCo2PerKwhHourOfElectricGenerated;
        }

        private double CalculateKilogramsOfCo2EmissionsPerMwhOfCogneratedElectric()
        {
            double value = _cogeneratedElectric.SteamEnergyUsed.MegabritishThermalUnits * CalculateKilogramsOfCo2PerMmbtuOfSteam() / _campusElectricGeneration.MegawattHours;
            return value;
        }

        private double CalculateKilogramsOfCo2EmissionsPerMwhOfElectric()
        {
            double value = (FractionElectricPurchasedFromGrid * CalculateKilogramsOfCo2EmissionsPerMwhOfGridElectric()) + (FractionElectricGeneratedOnCampus * CalculateKilogramsOfCo2EmissionsPerMwhOfCogneratedElectric());
            return value;
        }

        private double CalculateKilogramsOfCo2EmissionsPerGallonOfChilledWater()
        {
            Mass co2FromElectric = Mass.FromKilograms(_chilledWaterProduced.ElectricEnergyUsed.MegawattHours * CalculateKilogramsOfCo2EmissionsPerMwhOfElectric());
            Mass co2FromSteam = Mass.FromKilograms(_chilledWaterProduced.SteamEnergyUsed.MegabritishThermalUnits * CalculateKilogramsOfCo2PerMmbtuOfSteam());
            double value = (co2FromElectric + co2FromSteam).Kilograms / _chilledWaterProduced.ChilledWaterVolume.UsGallons;
            return value;
        }
    }
}
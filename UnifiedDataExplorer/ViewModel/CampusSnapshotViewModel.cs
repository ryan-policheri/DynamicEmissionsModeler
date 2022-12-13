using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EmissionsMonitorModel;

namespace UnifiedDataExplorer.ViewModel
{
    public class CampusSnapshotViewModel : ViewModelBase
    {
        private CampusSnapshot _snapshot;

        public CampusSnapshotViewModel(CampusSnapshot snapshot)
        {
            _snapshot = snapshot;

            Buildings = new ObservableCollection<BuildingUsageSummary>();
            foreach (BuildingUsageSummary summary in _snapshot.BuildingUsageSummaries)
            {
                Buildings.Add(summary);
            }

            CampusEnergyStats = new ObservableCollection<StatViewModel>();
            TotalEnergyStats = new ObservableCollection<StatViewModel>();
            ElectricEnergyStats = new ObservableCollection<StatViewModel>();
            HeatingEnergyStats = new ObservableCollection<StatViewModel>();
            CoolingEnergyStats = new ObservableCollection<StatViewModel>();

            SelectedBuilding = Buildings.FirstOrDefault();
            SelectedTime = _snapshot.End;

            StepBack = new DelegateCommand(OnStepBack);
            GoBeginning = new DelegateCommand(OnGoBegining);
            StepForward = new DelegateCommand(OnStepForward);
            GoEnd = new DelegateCommand(OnGoEnd);
        }

        public ObservableCollection<BuildingUsageSummary> Buildings { get; }

        private BuildingUsageSummary _selectedBuilding;
        public BuildingUsageSummary SelectedBuilding
        {
            get { return _selectedBuilding; }
            set 
            {
                SetField<BuildingUsageSummary>(ref _selectedBuilding, value);
                OnPropertyChanged(nameof(SquareFeetDisplay));
                SetCurrentBuildingUsage();
            }
        }

        public string SquareFeetDisplay => String.Format("{0:n0}", this.SelectedBuilding?.SquareFeet)  + " Square Feet";

        private DateTimeOffset _selectedTime;
        public DateTimeOffset SelectedTime
        {
            get { return _selectedTime; }
            set 
            { 
                SetField<DateTimeOffset>(ref _selectedTime, value);
                OnPropertyChanged(nameof(TimeDisplay));
                SetCurrentBuildingUsage();
            }
        }

        public string TimeDisplay
        {
            get 
            {
                string day;
                if (_selectedTime.LocalDateTime.Date == DateTime.Today) day = "Today";
                else if (_selectedTime.LocalDateTime.Date == DateTime.Today.AddDays(-1)) day = "Yesterday";
                else day = _selectedTime.LocalDateTime.ToLongDateString();

                string time = _selectedTime.LocalDateTime.ToString("h tt", CultureInfo.InvariantCulture);
                time = time.TrimEnd("AM").TrimEnd("PM").TrimEnd();
                string amPm = _selectedTime.LocalDateTime.ToString("tt", CultureInfo.InvariantCulture);

                string value = day + " from " + time + " " + amPm + " to " + time + ":59 " + amPm;
                return value;
            }
        }

        public ICommand StepBack { get; }

        public ICommand GoBeginning { get; }

        public ICommand StepForward { get; }

        public ICommand GoEnd { get; }


        private BuildingUsage _selectedBuildingUsage;
        public BuildingUsage SelectedBuildingUsage
        {
            get { return _selectedBuildingUsage; }
            set
            {
                SetField<BuildingUsage>(ref _selectedBuildingUsage, value);
                OnPropertyChanged(nameof(TotalGallonsDisplay));
                OnPropertyChanged(nameof(ElectricGallonsDisplay));
                OnPropertyChanged(nameof(HeatingGallonsDisplay));
                OnPropertyChanged(nameof(CoolingGallonsDisplay));
                OnPropertyChanged(nameof(TotalKilogramsDisplay));
                OnPropertyChanged(nameof(ElectricKilogramsDisplay));
                OnPropertyChanged(nameof(HeatingKilogramsDisplay));
                OnPropertyChanged(nameof(CoolingKilogramsDisplay));
            }
        }

        public ObservableCollection<StatViewModel> CampusEnergyStats { get; }
        public ObservableCollection<StatViewModel> TotalEnergyStats { get; }
        public ObservableCollection<StatViewModel> ElectricEnergyStats { get; }
        public ObservableCollection<StatViewModel> HeatingEnergyStats { get; }
        public ObservableCollection<StatViewModel> CoolingEnergyStats { get; }

        public string TotalGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.TotalCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";
        public string TotalKilogramsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.TotalCo2.Kilograms) + " Kilograms";
        public string ElectricGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.ElectricCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";
        public string ElectricKilogramsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.Co2FromElectricUsage.Kilograms) + " Kilograms";
        public string HeatingGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.HeatingCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";
        public string HeatingKilogramsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.Co2FromSteamUsage.Kilograms) + " Kilograms";
        public string CoolingGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.CoolingCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";
        public string CoolingKilogramsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.Co2FromChilledWaterUsage.Kilograms) + " Kilograms";


        private void SetCurrentBuildingUsage()
        {
            if (this.SelectedBuilding == null || this.SelectedTime == null || this.SelectedTime == default(DateTimeOffset)) return;
            SelectedBuildingUsage = this.SelectedBuilding.BuildingUsages.First(x => x.Timestamp == this.SelectedTime);

            HourDetails hourDetails = _snapshot.EnergyResources.First(x => x.Hour == this.SelectedTime);

            CampusEnergyStats.Clear();
            CampusEnergyStats.Add(new StatViewModel("Electric Purchased:", hourDetails.ElectricPurchased.MegawattHours, "mwh"));
            CampusEnergyStats.Add(new StatViewModel("Electric Generated:", hourDetails.CampusElectricGeneration.MegawattHours, "mwh"));
            CampusEnergyStats.Add(new StatViewModel("Steam Produced:", hourDetails.SteamProduced.MegabritishThermalUnits, "mmbtu"));
            CampusEnergyStats.Add(new StatViewModel("Chilled Water Produced:", hourDetails.ChilledWaterProduced.UsGallons, "gallons"));
            CampusEnergyStats.Add(new StatViewModel("Power Plant Electric Overhead:", hourDetails.PowerPlantElectricOverhead.MegawattHours, "mwh"));;
            CampusEnergyStats.Add(new StatViewModel("Power Plant Steam Overhead:", hourDetails.PowerPlantSteamOverhead.MegabritishThermalUnits, "mmbtu"));

            TotalEnergyStats.Clear();
            TotalEnergyStats.Add(new StatViewModel("CO2 from Energy Use", SelectedBuildingUsage.TotalCo2.Kilograms, "kg"));
            TotalEnergyStats.Add(new StatViewModel("CO2 per Square Foot", SelectedBuildingUsage.TotalCo2.Kilograms / this.SelectedBuilding.SquareFeet, "co2-kg per Sq. Ft"));
            TotalEnergyStats.Add(new StatViewModel("CO2 per Square Foot", SelectedBuildingUsage.TotalCo2InGasolineVolumeEquivelent.UsGallons / this.SelectedBuilding.SquareFeet, "gas-gallons per Sq. Ft"));

            ElectricEnergyStats.Clear();
            ElectricEnergyStats.Add(new StatViewModel("Electric Used:", SelectedBuildingUsage.ElectricUsage.KilowattHours, "kwh"));
            ElectricEnergyStats.Add(new StatViewModel("CO2 From Electric:", SelectedBuildingUsage.Co2FromElectricUsage.Kilograms, "kg"));
            ElectricEnergyStats.Add(new StatViewModel("Electric Emissions Factor", hourDetails.ElectricEmissionsFactorInKilogramsOfCo2PerMegawattHour, "kg-co2 per mwh"));
            ElectricEnergyStats.Add(new StatViewModel("Electric Purchased:", hourDetails.ElectricPurchased.MegawattHours, "mwh"));
            ElectricEnergyStats.Add(new StatViewModel("Electric Generated:", hourDetails.CampusElectricGeneration.MegawattHours, "mwh"));

            HeatingEnergyStats.Clear();
            HeatingEnergyStats.Add(new StatViewModel("Steam Used (energy):", SelectedBuildingUsage.SteamUsageAsEnergy.MegabritishThermalUnits, "mmbtu"));
            HeatingEnergyStats.Add(new StatViewModel("Steam Used (mass):", SelectedBuildingUsage.SteamUsageAsMass.Kilograms, "kg"));
            HeatingEnergyStats.Add(new StatViewModel("CO2 From Steam:", SelectedBuildingUsage.Co2FromSteamUsage.Kilograms, "kg"));
            HeatingEnergyStats.Add(new StatViewModel("Steam Emissions Factor", hourDetails.SteamEmissionsFactorInKilogramsOfCo2PerMmbtu, "kg-co2 per mmbtu"));

            CoolingEnergyStats.Clear();
            CoolingEnergyStats.Add(new StatViewModel("Chilled Water Used:", SelectedBuildingUsage.ChilledWaterUsage.UsGallons, "gallons"));
            CoolingEnergyStats.Add(new StatViewModel("CO2 From Chilled Water:", SelectedBuildingUsage.Co2FromChilledWaterUsage.Kilograms, "kg"));
            CoolingEnergyStats.Add(new StatViewModel("Chilled Water Emissions Factor", hourDetails.ChilledWaterEmissionsFactorInKilogramsOfCo2PerChilledWaterGallon, "kg-co2 per gallon"));
        }

        private void OnStepBack()
        {
            if(this.SelectedTime > this.SelectedBuilding.BuildingUsages.Min(x => x.Timestamp))
            {
                this.SelectedTime = this.SelectedTime.AddHours(-1);
            }
        }

        private void OnGoBegining()
        {
            this.SelectedTime = this.SelectedBuilding.BuildingUsages.Min(x => x.Timestamp);
        }

        private void OnStepForward()
        {
            if (this.SelectedTime < this.SelectedBuilding.BuildingUsages.Max(x => x.Timestamp))
            {
                this.SelectedTime = this.SelectedTime.AddHours(1);
            }
        }

        private void OnGoEnd()
        {
            this.SelectedTime = this.SelectedBuilding.BuildingUsages.Max(x => x.Timestamp);
        }
    }
}

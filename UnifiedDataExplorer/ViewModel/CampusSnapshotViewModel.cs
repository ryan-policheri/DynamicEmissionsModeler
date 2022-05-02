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
using UIowaBuildingsModel;

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

                string value = day + " at " + time;
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
            }
        }

        public string TotalGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.TotalCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";
        public string ElectricGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.ElectricCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";
        public string HeatingGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.HeatingCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";
        public string CoolingGallonsDisplay => String.Format("{0:n0}", SelectedBuildingUsage.CoolingCo2InGasolineVolumeEquivelent.UsGallons) + " Gasoline Gallons";

        private void SetCurrentBuildingUsage()
        {
            if (this.SelectedBuilding == null || this.SelectedTime == null || this.SelectedTime == default(DateTimeOffset)) return;
            SelectedBuildingUsage = this.SelectedBuilding.BuildingUsages.First(x => x.Timestamp == this.SelectedTime);
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

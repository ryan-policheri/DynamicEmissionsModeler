using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using PiModel;
using EmissionsMonitorModel;

namespace UnifiedDataExplorer.ViewModel
{
    public class HourlyEmissionsReportParametersViewModel : ViewModelBase
    {
        private const string SELECT_ALL = "Select All";
        private const string UNSELECT_ALL = "Unselect All";

        private string[] exludeBuildings = { "College of Medicine Administration Building", "Dey House", "Distribution", "Engineering Research Facility",
                                             "Hydraulics Laboratory Wind Tunnel", "Hydraulics Mode Annex", "Landscape Services Complex",
                                             "North Campus Parking and Chilled Water Facility", "Presidents Residence", "Quadrangle Hall",
                                             "Stanley Museum of Art", "State Historical Society Building", "West Campus Steam Plant" };

        public HourlyEmissionsReportParametersViewModel(HourlyEmissionsReportParameters model, IEnumerable<Asset> availableAssets)
        {
            StartDate = model.StartDateInLocalTime;
            EndDate = model.EndDateInLocalTime;
            GenerateExcel = true;
            GenerateDashboard = true;
            SelectAllCommand = new DelegateCommand(OnSelectAll);
            SelectAllText = SELECT_ALL;
            AvailableAssets = new ObservableCollection<SelectableAssetViewModel>();
            foreach(var asset in availableAssets)
            {
                if(!asset.Name.OneOf(exludeBuildings))
                {
                    SelectableAssetViewModel vm = new SelectableAssetViewModel(asset);
                    if (model.AssetLinks.Contains(asset.Links.Self)) vm.IsSelected = true;
                    AvailableAssets.Add(vm);
                }
            }
        }

        private DateTime _startTime;
        public DateTime StartDate
        {
            get { return _startTime; }
            set
            {
                if (value.Kind == DateTimeKind.Utc) throw new ArgumentException("Local or unspecified time expected"); //TODO: Do this in a converter
                if (value.Kind == DateTimeKind.Unspecified) value = new DateTime(value.Ticks, DateTimeKind.Local);
                SetField<DateTime>(ref _startTime, value); 
            }
        }

        private DateTime _endTime;
        public DateTime EndDate
        {
            get { return _endTime; }
            set
            {
                if (value.Kind == DateTimeKind.Utc) throw new ArgumentException("Local or unspecified time expected"); // TODO: Do this in a converter
                if (value.Kind == DateTimeKind.Unspecified) value = new DateTime(value.Ticks, DateTimeKind.Local);
                SetField<DateTime>(ref _endTime, value); 
            }
        }

        public ICommand SelectAllCommand { get; }

        private string _selectAllText;
        public string SelectAllText
        {
            get { return _selectAllText; }
            private set { SetField<string>(ref _selectAllText, value); }
        }

        public ObservableCollection<SelectableAssetViewModel> AvailableAssets { get; }
        public ElectricGridStrategy GridStrategy { get; set; } = ElectricGridStrategy.MisoHourly;

        public bool GenerateExcel { get; set; }

        public bool GenerateDashboard { get; set; }

        public bool DoExecute { get; set; }

        public HourlyEmissionsReportParameters ToModel()
        {
            HourlyEmissionsReportParameters model = new HourlyEmissionsReportParameters();
            model.StartDateInLocalTime = this.StartDate;
            model.EndDateInLocalTime = this.EndDate;
            model.GridStrategy = this.GridStrategy;
            foreach(var asset in this.AvailableAssets)
            {
                if(asset.IsSelected)
                {
                    model.AssetLinks.Add(asset.GetBackingModel().Links.Self);
                }
            }
            return model;
        }

        private void OnSelectAll()
        {
            foreach (var asset in this.AvailableAssets)
            {
                asset.IsSelected = SelectAllText == SELECT_ALL;
            }
            if (SelectAllText == SELECT_ALL) SelectAllText = UNSELECT_ALL;
            else SelectAllText = SELECT_ALL;
        }
    }

    public class SelectableAssetViewModel : ViewModelBase
    {
        private readonly Asset _asset;

        public SelectableAssetViewModel(Asset asset)
        {
            _asset = asset;
        }

        public string Name => _asset.Name;

        private bool _isSelected;
        public bool IsSelected 
        {
            get { return _isSelected; }
            set { SetField<bool>(ref _isSelected, value); }
        }

        public Asset GetBackingModel() => _asset;
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.MVVM;
using PiModel;
using UIowaBuildingsModel;

namespace UnifiedDataExplorer.ViewModel
{
    public class HourlyEmissionsReportParametersViewModel : ViewModelBase
    {
        private const string SELECT_ALL = "Select All";
        private const string UNSELECT_ALL = "Unselect All";

        public HourlyEmissionsReportParametersViewModel(HourlyEmissionsReportParameters model, IEnumerable<Asset> availableAssets)
        {
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            SelectAllCommand = new DelegateCommand(OnSelectAll);
            SelectAllText = SELECT_ALL;
            AvailableAssets = new ObservableCollection<SelectableAssetViewModel>();
            foreach(var asset in availableAssets)
            {
                SelectableAssetViewModel vm = new SelectableAssetViewModel(asset);
                if(model.AssetLinks.Contains(asset.Links.Self)) vm.IsSelected = true;
                AvailableAssets.Add(vm);
            }
        }

        private DateTime _startTime;
        public DateTime StartDate
        {
            get { return _startTime; }
            set { SetField<DateTime>(ref _startTime, value); }
        }

        private DateTime _endTime;
        public DateTime EndDate
        {
            get { return _endTime; }
            set { SetField<DateTime>(ref _endTime, value); }
        }

        public ICommand SelectAllCommand { get; }

        private string _selectAllText;
        public string SelectAllText
        {
            get { return _selectAllText; }
            private set { SetField<string>(ref _selectAllText, value); }
        }

        public ObservableCollection<SelectableAssetViewModel> AvailableAssets { get; }

        public HourlyEmissionsReportParameters ToModel()
        {
            HourlyEmissionsReportParameters model = new HourlyEmissionsReportParameters();
            model.StartDate = this.StartDate;
            model.EndDate = this.EndDate;
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
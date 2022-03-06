using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DotNetCommon.MVVM;
using PiModel;
using UIowaBuildingsModel;

namespace UnifiedDataExplorer.ViewModel
{
    public class HourlyEmissionsReportParametersViewModel : ViewModelBase
    {
        public HourlyEmissionsReportParametersViewModel(HourlyEmissionsReportParameters model, IEnumerable<Asset> availableAssets)
        {
            StartDate = model.StartDate;
            EndDate = model.EndDate;
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
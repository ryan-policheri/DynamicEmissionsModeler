using System;
using System.Collections.ObjectModel;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class FunctionFactorViewModel : ViewModelBase, IHaveDataStatus
    {
        private readonly Func<int, DataSourceBase> _dataSourceNameResolver;
        private readonly FunctionFactor _model;

        public FunctionFactorViewModel(Func<int, DataSourceBase> dataSourceNameResolver, FunctionFactor factor)
        {
            _dataSourceNameResolver = dataSourceNameResolver;
            _model = factor;
            AvailableUnitRates = new ObservableCollection<string>();
            foreach(var r in UnitRates.ToListing()) { AvailableUnitRates.Add(r); }
            AvailableDataResolutions = new ObservableCollection<string>();
            foreach (var r in DataResolutionPlusVariable.ToListing()) { AvailableDataResolutions.Add(r); }
        }

        public string FactorName
        {
            get { return _model.FactorName; }
            set { _model.FactorName = value; OnPropertyChanged(); OnPropertyChanged(nameof(ParameterName)); }
        }

        public string ParameterName => _model.ParameterName;
        public string DataSourceName => _dataSourceNameResolver(_model.FactorUri.DataSourceId).SourceName;
        public string SeriesName => _model.FactorUri?.SeriesName;
        public string UnitsSummary => _model.FactorUri?.SeriesUnitsSummary;
        public ObservableCollection<string> AvailableUnitRates { get; }
        public string SelectedUnitRate
        {
            get { return _model.FactorUri?.SeriesUnitRate; }
            set { if (_model.FactorUri != null) _model.FactorUri.SeriesUnitRate = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> AvailableDataResolutions { get; }
        public string SelectedDataResolution
        {
            get { return _model.FactorUri?.SeriesDataResolution; }
            set { if (_model.FactorUri != null) _model.FactorUri.SeriesDataResolution = value; OnPropertyChanged(); }
        }


        public bool ShowDropZone => _model.FactorUri == null;

        public bool ShowSeriesInfo => _model.FactorUri != null;

        public ViewModelDataStatus Status { get; set; }

        public FunctionFactor GetBackingModel() => _model;

        public void SetSeries(DataSourceSeriesUri seriesUri)
        {
            _model.FactorUri = seriesUri;
            OnPropertyChanged(nameof(DataSourceName));
            OnPropertyChanged(nameof(SeriesName));
            OnPropertyChanged(nameof(UnitsSummary));
            OnPropertyChanged(nameof(ShowDropZone));
            OnPropertyChanged(nameof(ShowSeriesInfo));
        }
    }
}

using DotNetCommon.MVVM;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class FunctionFactorViewModel : ViewModelBase, IHaveDataStatus
    {
        private readonly FunctionFactor _model;

        public FunctionFactorViewModel(FunctionFactor factor)
        {
            _model = factor;
        }

        public string FactorName
        {
            get { return _model.FactorName; }
            set { _model.FactorName = value; OnPropertyChanged(); OnPropertyChanged(nameof(ParameterName)); }
        }

        public string ParameterName => _model.ParameterName;

        public string DataSourceName => "UIowa Energy System"; //TODO
        public string SeriesName => _model.FactorUri?.SeriesName;

        public bool ShowDropZone => _model.FactorUri == null;

        public bool ShowSeriesInfo => _model.FactorUri != null;

        public ViewModelDataStatus Status { get; set; }

        public FunctionFactor GetBackingModel() => _model;

        public void SetSeries(DataSourceSeriesUri seriesUri)
        {
            _model.FactorUri = seriesUri;
            OnPropertyChanged(nameof(SeriesName));
            OnPropertyChanged(nameof(ShowDropZone));
            OnPropertyChanged(nameof(ShowSeriesInfo));
        }
    }
}

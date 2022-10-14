using DotNetCommon.MVVM;
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
            get { return _model?.FactorName; }
            set { if(_model != null) _model.FactorName = value; OnPropertyChanged(); }
        }

        public ViewModelDataStatus Status { get; set; }
    }
}

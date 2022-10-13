using DotNetCommon.MVVM;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public abstract class ProcessNodeViewModel : ViewModelBase
    {
        private readonly ProcessNode _model;

        public ProcessNodeViewModel(ProcessNode processNode)
        {
            _model = processNode;
        }

        public abstract string NodeTypeName { get; }

        public string NodeName
        { 
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged(); }
        }
    }
}

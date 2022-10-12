using DotNetCommon.MVVM;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ProcessNodeViewModel : ViewModelBase
    {
        private readonly ProcessNode _model;

        public ProcessNodeViewModel(ProcessNode processNode)
        {
            _model = processNode;
        }

        public string NodeName
        { 
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged(); }
        }
    }
}

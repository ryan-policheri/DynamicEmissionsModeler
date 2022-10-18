using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public abstract class ProcessNodeViewModel : RobustViewModelBase
    {
        private ProcessNode _model;

        protected ProcessNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
        }

        public abstract string NodeTypeName { get; }

        public string NodeName
        { 
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged(); }
        }

        public virtual void Load(ProcessNode node)
        {
            _model = node;
        }
    }
}

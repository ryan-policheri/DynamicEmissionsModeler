using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ProcessNodesViewModel : RobustViewModelBase
    {
        private ProcessModel _model;

        public ProcessNodesViewModel(NodesNavigationViewModel nodeNavViewModel, RobustViewModelDependencies facade) : base(facade)
        {
            NodesNavigationViewModel = nodeNavViewModel;
            NodesNavigationViewModel.PropertyChanged += NodesNavigationViewModel_PropertyChanged;
        }

        public void Load(ProcessModel model)
        {
            _model = model;
            NodesNavigationViewModel.Load(_model);
        }

        public NodesNavigationViewModel NodesNavigationViewModel { get; }

        public ProcessNodeViewModel SelectedProcessNode { get; private set; }

        private void NodesNavigationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(NodesNavigationViewModel.SelectedProcessNode))
            {
                SelectedProcessNode = NodesNavigationViewModel.SelectedProcessNode; OnPropertyChanged(nameof(SelectedProcessNode));
            }
        }
    }
}

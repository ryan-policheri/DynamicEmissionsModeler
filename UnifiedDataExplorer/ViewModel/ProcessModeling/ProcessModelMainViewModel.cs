using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ProcessModelMainViewModel : RobustViewModelBase
    {
        public ProcessModelMainViewModel(NodesNavigationViewModel nodeNavViewModel, RobustViewModelDependencies facade) : base(facade)
        {
            NodesNavigationViewModel = nodeNavViewModel;
            NodesNavigationViewModel.PropertyChanged += NodesNavigationViewModel_PropertyChanged;
            SaveCommand = new DelegateCommand<bool?>(OnSave);
        }

        public NodesNavigationViewModel NodesNavigationViewModel { get; }

        public ProcessNodeViewModel SelectedProcessNode { get; private set; }

        public ICommand SaveCommand { get; }

        private void NodesNavigationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(NodesNavigationViewModel.SelectedProcessNode))
            {
                SelectedProcessNode = NodesNavigationViewModel.SelectedProcessNode; OnPropertyChanged(nameof(SelectedProcessNode));
            }
        }

        private void OnSave(bool? obj)
        {
            //EnergyModelSaveItem 
        }
    }
}

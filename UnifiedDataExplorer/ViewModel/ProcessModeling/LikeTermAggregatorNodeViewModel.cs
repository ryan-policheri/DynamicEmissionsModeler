using System.Collections.ObjectModel;
using System.Linq;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class LikeTermAggregatorNodeViewModel : ProcessNodeViewModel
    {
        private ProcessModel _processModel;
        private LikeTermsAggregatorNode _aggregatorModel;

        public LikeTermAggregatorNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<CheckableItemViewModel>();
        }

        public override string NodeTypeName => "Like Term Aggregator Node";

        public void Load(ProcessNode aggregatorNode, ProcessModel model)
        {
            base.Load(aggregatorNode);
            _aggregatorModel = aggregatorNode as LikeTermsAggregatorNode;
            _processModel = model;
            
            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in _processModel.CalculateDerivedSeries().Where(x => x.Id > 0 && x.Id != _aggregatorModel.Id))
            {
                var vm = new CheckableItemViewModel
                {
                    IsChecked = _aggregatorModel.PrecedingNodeIds.Contains(spec.Id),
                    Item = spec
                };
                vm.PropertyChanged += OnCheckableItemChanged;
                AvailableNodeOutputSpecs.Add(vm);
            }
        }

        private void OnCheckableItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CheckableItemViewModel.IsChecked))
            {
                CheckableItemViewModel vm = sender as CheckableItemViewModel;
                if(vm.IsChecked && !_aggregatorModel.PrecedingNodeIds.Contains(vm.ItemId)) _aggregatorModel.PrecedingNodeIds.Add(vm.ItemId);
                else if (!vm.IsChecked && _aggregatorModel.PrecedingNodeIds.Contains(vm.ItemId)) _aggregatorModel.PrecedingNodeIds.Remove(vm.ItemId);
            }
        }

        public ObservableCollection<CheckableItemViewModel> AvailableNodeOutputSpecs { get; }
    }
}

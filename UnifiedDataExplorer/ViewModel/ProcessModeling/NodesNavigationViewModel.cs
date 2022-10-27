using System;
using System.Collections.ObjectModel;
using System.Linq;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class NodesNavigationViewModel : RobustViewModelBase
    {
        private const string UNSELECTED = "(Select node to add)";
        private const string EXCHANGE_NODE = "Exchange Node";
        private const string LIKE_TERM_AGGREGATOR = "Like Term Aggregator Node";

        private ProcessModel _model;

        public NodesNavigationViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AddOptions = new ObservableCollection<string>();
            ProcessNodes = new ObservableCollection<ProcessNodeViewModel>();
            ResetAddOptions();
        }

        public void Load(ProcessModel model)
        {
            _model = model;
            foreach (ProcessNode node in _model.ProcessNodes) WrapAndAddNode(node);
        }

        public ObservableCollection<string> AddOptions { get; }
        private string _selectedAddOption;
        public string SelectedAddOption
        {
            get { return _selectedAddOption; }
            set
            {
                SetField(ref _selectedAddOption, value);
                if (value == EXCHANGE_NODE) CreateAndWrapNode<ExchangeNode>();
                if (value == LIKE_TERM_AGGREGATOR) CreateAndWrapNode<LikeTermsAggregatorNode>();
            }
        }


        public ObservableCollection<ProcessNodeViewModel> ProcessNodes { get; }

        private ProcessNodeViewModel _selectedProcessNode;
        public ProcessNodeViewModel SelectedProcessNode
        {
            get { return _selectedProcessNode; }
            set { SetField(ref _selectedProcessNode, value); }
        }

        public void RemoveProcessNode(ProcessNodeViewModel processNode)
        {
            this._model.ProcessNodes.Remove(processNode.GetBackingModel());
            this.ProcessNodes.Remove(processNode);
            this.SelectedProcessNode = null;
        }

        private void CreateAndWrapNode<T>() where T : ProcessNode
        {
            T node = Activator.CreateInstance<T>();
            node.Name = "New " + typeof(T).Name;
            _model.AddProcessNode(node);
            WrapAndAddNode(node);
            ResetAddOptions();
        }

        private void WrapAndAddNode(ProcessNode node)
        {
            ProcessNodeViewModel vm;
            switch (node.GetType().Name)
            {
                case nameof(ExchangeNode):
                    vm = this.Resolve<ExchangeNodeViewModel>();
                    break;
                case nameof(LikeTermsAggregatorNode):
                    vm = this.Resolve<LikeTermAggregatorNodeViewModel>();
                    break;
                default:
                    throw new NotImplementedException(
                        $"Do not know what viewmodel to use for the given node of type {node.GetType().Name}");
            }

            vm.Load(node);
            ProcessNodes.Add(vm);
        }

        private void ResetAddOptions()
        {
            AddOptions.Clear();
            AddOptions.Add(UNSELECTED); AddOptions.Add(EXCHANGE_NODE); AddOptions.Add(LIKE_TERM_AGGREGATOR);
            SelectedAddOption = AddOptions.First();
        }
    }
}

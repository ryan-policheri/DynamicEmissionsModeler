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
            foreach (ProcessNode node in _model.ProcessNodes)
            {
                //ProcessNodes.Add(node);
            }
        }

        public ObservableCollection<string> AddOptions { get; }
        private string _selectedAddOption;
        public string SelectedAddOption
        {
            get { return _selectedAddOption; }
            set
            {
                SetField(ref _selectedAddOption, value);
                if (value == EXCHANGE_NODE) AddExchangeNode();
                if (value == LIKE_TERM_AGGREGATOR) AddLikeTermAggregator();
            }
        }


        public ObservableCollection<ProcessNodeViewModel> ProcessNodes { get; }

        private ProcessNodeViewModel _selectedProcessNode;
        public ProcessNodeViewModel SelectedProcessNode
        {
            get { return _selectedProcessNode; }
            set { SetField(ref _selectedProcessNode, value); }
        }

        private void AddExchangeNode()
        {
            ExchangeNode node = new ExchangeNode();
            _model.AddProcessNode(node);
            ExchangeNodeViewModel vm = this.Resolve<ExchangeNodeViewModel>();
            vm.Load(node);
            vm.NodeName = "New Exchange Node";
            ProcessNodes.Add(vm);
            SelectedProcessNode = vm;
            SelectedAddOption = AddOptions.First(x => x == UNSELECTED);
            ResetAddOptions();
        }


        private void AddLikeTermAggregator()
        {
            LikeTermsAggregatorNode node = new LikeTermsAggregatorNode();
            _model.AddProcessNode(node);
            LikeTermAggregatorNodeViewModel vm = this.Resolve<LikeTermAggregatorNodeViewModel>();
            vm.Load(node);
            vm.NodeName = "New Like Term Aggregator Node";
            ProcessNodes.Add(vm);
            SelectedProcessNode = vm;
            ResetAddOptions();
        }

        private void ResetAddOptions()
        {
            AddOptions.Clear();
            AddOptions.Add(UNSELECTED); AddOptions.Add(EXCHANGE_NODE); AddOptions.Add(LIKE_TERM_AGGREGATOR);
            SelectedAddOption = AddOptions.First();
        }
    }
}

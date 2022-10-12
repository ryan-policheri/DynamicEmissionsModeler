using DotNetCommon.DelegateCommand;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class NodesNavigationViewModel : RobustViewModelBase
    {
        private const string UNSELECTED = "(Select node to add)";
        private const string EXCHANGE_NODE = "Exchange Node";
        private const string LIKE_TERM_AGGREGATOR = "Like Term Aggregator Node";

        public NodesNavigationViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AddOptions = new ObservableCollection<string>();
            ProcessNodes = new ObservableCollection<ProcessNodeViewModel>();
            ResetAddOptions();
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
            ExchangeNodeViewModel vm = new (new ExchangeNode());
            vm.NodeName = "New Exchange Node";
            ProcessNodes.Add(vm);
            SelectedProcessNode = vm;
            SelectedAddOption = AddOptions.First(x => x == UNSELECTED);
            ResetAddOptions();
        }


        private void AddLikeTermAggregator()
        {
            LikeTermAggregatorViewModel vm = new LikeTermAggregatorViewModel(new LikeTermsAggregatorNode());
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

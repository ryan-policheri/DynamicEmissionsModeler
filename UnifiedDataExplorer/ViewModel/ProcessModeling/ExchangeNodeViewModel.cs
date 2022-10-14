using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ExchangeNodeViewModel : ProcessNodeViewModel
    {
        private ExchangeNode _model;

        public ExchangeNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {

            CostFunctions = new ObservableCollection<DataFunctionViewModel>();
            AddCostFunction = new DelegateCommand(OnAddCostFunction);
        }

        public override string NodeTypeName => "Exchange Node";

        public ObservableCollection<DataFunctionViewModel> CostFunctions { get; }

        private DataFunctionViewModel _selectedCostFunction;
        public DataFunctionViewModel SelectedCostFunction
        {
            get { return _selectedCostFunction; }
            set { SetField(ref _selectedCostFunction, value); }
        }
        
        public ICommand AddCostFunction { get; }

        public void Load(ExchangeNode exchangeNode)
        {
            base.Load(exchangeNode);
            _model = exchangeNode;

            foreach (DataFunction function in exchangeNode.Costs)
            {
                var vm = this.Resolve<DataFunctionViewModel>();
                vm.Load(function);
                CostFunctions.Add(vm);
            }
        }

        private void OnAddCostFunction()
        {
            var vm = this.Resolve<DataFunctionViewModel>();
            vm.Load(null);
            CostFunctions.Add(vm);
            SelectedCostFunction = vm;
        }
    }
}

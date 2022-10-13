using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ExchangeNodeViewModel : ProcessNodeViewModel
    {
        private readonly ExchangeNode _model;

        public ExchangeNodeViewModel(ExchangeNode exchangeNode) : base(exchangeNode)
        {
            _model = exchangeNode;
            CostFunctions = new ObservableCollection<DataFunctionViewModel>();
            AddCostFunction = new DelegateCommand(OnAddCostFunction);

            foreach (DataFunction function in exchangeNode.Costs)
            {
                CostFunctions.Add(new DataFunctionViewModel(function));
            }
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

        private void OnAddCostFunction()
        {
            DataFunctionViewModel newFunc = new DataFunctionViewModel(null);
            CostFunctions.Add(newFunc);
            SelectedCostFunction = newFunc;
        }
    }
}

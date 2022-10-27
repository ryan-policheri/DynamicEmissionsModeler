using System.Collections.ObjectModel;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ExchangeNodeViewModel : ProcessNodeViewModel
    {
        private ExchangeNode _model;

        public ExchangeNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {

            CostFunctions = new ObservableCollection<DataFunction>();
            AddCostFunction = new DelegateCommand(OnAddCostFunction);
        }

        public override string NodeTypeName => "Exchange Node";

        public ObservableCollection<DataFunction> CostFunctions { get; }

        private DataFunction _selectedCostFunction;
        public DataFunction SelectedCostFunction
        {
            get { return _selectedCostFunction; }
            set
            {
                SetField(ref _selectedCostFunction, value);
                OnCostFunctionSelected(value);
            }
        }

        private DataFunctionViewModel _currentCostFunctionViewModel;
        public DataFunctionViewModel CurrentCostFunctionViewModel
        {
            get { return _currentCostFunctionViewModel; }
            set { SetField(ref _currentCostFunctionViewModel, value); }
        }

        public ICommand AddCostFunction { get; }


        private DataFunctionViewModel _productFunctionViewModel;
        public DataFunctionViewModel ProductFunctionViewModel
        {
            get { return _productFunctionViewModel; }
            set { SetField(ref _productFunctionViewModel, value); }
        }

        public override void Load(ProcessNode exchangeNode)
        {
            base.Load(exchangeNode);
            _model = (ExchangeNode)exchangeNode;

            CostFunctions.Clear();
            foreach (DataFunction function in _model.Costs)
            {
                CostFunctions.Add(function);
            }

            DataFunctionViewModel vm = this.Resolve<DataFunctionViewModel>();
            vm.Load(_model.Product, null);
            ProductFunctionViewModel = vm;
        }

        private void OnCostFunctionSelected(DataFunction function)
        {
            DataFunction copy = function?.Copy();
            DataFunctionViewModel vm = this.Resolve<DataFunctionViewModel>();

            vm.Load(copy, (status) =>
            {
                if (status == ViewModelDataStatus.Added) { this._model.Costs.Add(vm.GetBackingModel()); }
                if (status == ViewModelDataStatus.Updated) { this._model.Costs.Remove(function); this._model.Costs.Add(vm.GetBackingModel()); }

                if (status == ViewModelDataStatus.Removed) { this._model.Costs.Remove(function); }
                if (status == ViewModelDataStatus.Canceled) { /*Do nothing*/ }

                Load(_model);
                this.SelectedCostFunction = null;
                this.CurrentCostFunctionViewModel = null;
            });

            this.CurrentCostFunctionViewModel = vm;
        }

        private void OnAddCostFunction()
        {
            OnCostFunctionSelected(null);
        }
    }
}

using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using EmissionsMonitorModel.ProcessModeling;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class MultiSplitterNodeViewModel : ProcessNodeViewModel
    {
        private ProcessModel _processModel;
        private MultiSplitterNode _multiSplitterModel;

        public MultiSplitterNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<NodeOutputSpec>();
            AddSplitFunction = new DelegateCommand(OnAddSplitFunction);
            SplitFunctions = new ObservableCollection<DataFunction>();
        }

        public ICommand AddSplitFunction { get; }

        public override string NodeTypeName => "Multi Splitter Node";

        public override string NodeImagePath => "/Resources/NodeIcons/MultiSplitterNode.png";

        public ObservableCollection<NodeOutputSpec> AvailableNodeOutputSpecs { get; }

        private NodeOutputSpec _selectedNodeToSplit;
        public NodeOutputSpec SelectedNodeToSplit
        {
            get { return _selectedNodeToSplit; }
            set
            {
                SetField(ref _selectedNodeToSplit, value);
                if (_selectedNodeToSplit != null) _multiSplitterModel.PrecedingNodeId = _selectedNodeToSplit.Id;
            }
        }

        public ObservableCollection<DataFunction> SplitFunctions { get; }

        private DataFunction _selectedSplitFunction;
        public DataFunction SelectedSplitFunction
        {
            get { return _selectedSplitFunction; }
            set
            {
                SetField(ref _selectedSplitFunction, value);
                OnSplitFunctionSelected(value);
            }
        }

        private DataFunctionViewModel _currentSplitFunctionViewModel;
        public DataFunctionViewModel CurrentSplitFunctionViewModel
        {
            get { return _currentSplitFunctionViewModel; }
            set { SetField(ref _currentSplitFunctionViewModel, value); }
        }

        public void Load(ProcessNode multiSplitterNode, ProcessModel processModel)
        {
            base.Load(multiSplitterNode);
            _multiSplitterModel = (MultiSplitterNode)multiSplitterNode;
            _processModel = processModel;

            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in _processModel.GetAllNodeSpecs().Where(x => x.Id > 0 && x.Id != _multiSplitterModel.Id))
            {
                AvailableNodeOutputSpecs.Add(spec);
            }
            SelectedNodeToSplit = AvailableNodeOutputSpecs.FirstOrDefault(x => x.Id == _multiSplitterModel.PrecedingNodeId);

            SplitFunctions.Clear();
            foreach (DataFunction function in _multiSplitterModel.SplitFunctions)
            {
                SplitFunctions.Add(function);
            }
        }

        private void OnSplitFunctionSelected(DataFunction function)
        {
            DataFunction copy = function?.Copy();
            DataFunctionViewModel vm = this.Resolve<DataFunctionViewModel>();

            vm.Load(copy, (status) =>
            {
                if (status == ViewModelDataStatus.Added) { this._multiSplitterModel.AddSplitFunction(vm.GetBackingModel()); }
                if (status == ViewModelDataStatus.Updated) { this._multiSplitterModel.UpdateSplitFunction(function, vm.GetBackingModel()); }

                if (status == ViewModelDataStatus.Removed) { this._multiSplitterModel.RemoveSplitFunction(function); }
                if (status == ViewModelDataStatus.Canceled) { /*Do nothing*/ }

                Load(_multiSplitterModel, _processModel);
                this.SelectedSplitFunction = null;
                this.CurrentSplitFunctionViewModel = null;
            });

            this.CurrentSplitFunctionViewModel = vm;
        }


        private void OnAddSplitFunction()
        {
            OnSplitFunctionSelected(null);
        }
    }
}

using EmissionsMonitorModel.ProcessModeling;
using System.Collections.ObjectModel;
using System.Linq;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class StreamSplitterNodeViewModel : ProcessNodeViewModel
    {
        private ProcessModel _processModel;
        private StreamSplitterNode _splitterModel;

        public StreamSplitterNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<NodeOutputSpec>();
        }

        public override string NodeTypeName => "Splitter Node";

        public ObservableCollection<NodeOutputSpec> AvailableNodeOutputSpecs { get; }

        private NodeOutputSpec _selectedNodeToSplit;
        public NodeOutputSpec SelectedNodeToSplit
        {
            get { return _selectedNodeToSplit; }
            set 
            {
                SetField(ref _selectedNodeToSplit, value);
                if (_selectedNodeToSplit != null) _splitterModel.PrecedingNodeId = _selectedNodeToSplit.Id;
            }
        }

        private DataFunctionViewModel _splitFunctionViewModel;
        public DataFunctionViewModel SplitFunctionViewModel
        {
            get { return _splitFunctionViewModel; }
            set { SetField(ref _splitFunctionViewModel, value); }
        }

        public void Load(ProcessNode splitterNode, ProcessModel processModel)
        {
            base.Load(splitterNode);
            _splitterModel = (StreamSplitterNode)splitterNode;
            _processModel = processModel;

            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in _processModel.CalculateDerivedSeries().Where(x => x.Id > 0 && x.Id != _splitterModel.Id))
            {
                AvailableNodeOutputSpecs.Add(spec);
            }
            SelectedNodeToSplit = AvailableNodeOutputSpecs.FirstOrDefault(x => x.Id == _splitterModel.PrecedingNodeId);

            DataFunctionViewModel vm = this.Resolve<DataFunctionViewModel>();
            vm.Load(_splitterModel.SplitFunction, null);
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DataFunctionViewModel.SelectedUnitForm))
                {
                    _splitterModel.SplitFunction = vm.GetBackingModel();
                }
            };
            SplitFunctionViewModel = vm;
        }
    }
}

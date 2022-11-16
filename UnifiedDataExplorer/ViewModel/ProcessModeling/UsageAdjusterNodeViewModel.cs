using EmissionsMonitorModel.ProcessModeling;
using System.Collections.ObjectModel;
using System.Linq;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class UsageAdjusterNodeViewModel : ProcessNodeViewModel
    {
        private ProcessModel _processModel;
        private UsageAdjusterNode _adjusterModel;

        public UsageAdjusterNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<NodeOutputSpec>();
        }

        public override string NodeTypeName => "Usage Adjuster Node";

        public ObservableCollection<NodeOutputSpec> AvailableNodeOutputSpecs { get; }


        private NodeOutputSpec _selectedNodeToAdjust;
        public NodeOutputSpec SelectedNodeToAdjust
        {
            get { return _selectedNodeToAdjust; }
            set
            {
                SetField(ref _selectedNodeToAdjust, value);
                if (_selectedNodeToAdjust != null) _adjusterModel.PrecedingNodeId = _selectedNodeToAdjust.Id;
            }
        }

        private DataFunctionViewModel _usageAdjusterFunctionViewModel;
        public DataFunctionViewModel UsageAdjusterFunctionViewModel
        {
            get { return _usageAdjusterFunctionViewModel; }
            set { SetField(ref _usageAdjusterFunctionViewModel, value); }
        }

        public void Load(ProcessNode adjusterNode, ProcessModel processModel)
        {
            base.Load(adjusterNode);
            _adjusterModel = (UsageAdjusterNode)adjusterNode;
            _processModel = processModel;

            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in _processModel.CalculateDerivedSeries().Where(x => x.Id > 0 && x.Id != _adjusterModel.Id))
            {
                AvailableNodeOutputSpecs.Add(spec);
            }
            SelectedNodeToAdjust = AvailableNodeOutputSpecs.FirstOrDefault(x => x.Id == _adjusterModel.PrecedingNodeId);

            DataFunctionViewModel vm = this.Resolve<DataFunctionViewModel>();
            vm.Load(_adjusterModel.ProductUsageFunction, null);
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DataFunctionViewModel.SelectedUnitForm))
                {
                    _adjusterModel.ProductUsageFunction = vm.GetBackingModel();
                }
            };
            UsageAdjusterFunctionViewModel = vm;
        }
    }
}

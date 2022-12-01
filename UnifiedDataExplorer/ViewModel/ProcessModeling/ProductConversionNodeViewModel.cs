using EmissionsMonitorModel.ProcessModeling;
using System.Collections.ObjectModel;
using System.Linq;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    internal class ProductConversionNodeViewModel : ProcessNodeViewModel
    {
        private ProcessModel _processModel;
        private ProductConversionNode _conversionModel;

        public ProductConversionNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<NodeOutputSpec>();
        }

        public override string NodeTypeName => "Product Conversion Node";

        public ObservableCollection<NodeOutputSpec> AvailableNodeOutputSpecs { get; }

        private NodeOutputSpec _selectedNodeToSplit;
        public NodeOutputSpec SelectedNodeToConvert
        {
            get { return _selectedNodeToSplit; }
            set
            {
                SetField(ref _selectedNodeToSplit, value);
                if (_selectedNodeToSplit != null) _conversionModel.PrecedingNodeId = _selectedNodeToSplit.Id;
            }
        }

        private DataFunctionViewModel _newProductFunctionViewModel;
        public DataFunctionViewModel NewProductFunctionViewModel
        {
            get { return _newProductFunctionViewModel; }
            set { SetField(ref _newProductFunctionViewModel, value); }
        }


        public void Load(ProcessNode conversionNode, ProcessModel processModel)
        {
            base.Load(conversionNode);
            _conversionModel = (ProductConversionNode)conversionNode;
            _processModel = processModel;

            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in _processModel.GetAllNodeSpecs().Where(x => x.Id > 0 && x.Id != _conversionModel.Id))
            {
                AvailableNodeOutputSpecs.Add(spec);
            }
            SelectedNodeToConvert = AvailableNodeOutputSpecs.FirstOrDefault(x => x.Id == _conversionModel.PrecedingNodeId);

            DataFunctionViewModel vm = this.Resolve<DataFunctionViewModel>();
            vm.Load(_conversionModel.NewProductFunction, null);
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DataFunctionViewModel.SelectedUnitForm))
                {
                    _conversionModel.NewProductFunction = vm.GetBackingModel();
                }
            };
            NewProductFunctionViewModel = vm;
        }
    }
}

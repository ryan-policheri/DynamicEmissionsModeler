using EmissionsMonitorModel.ProcessModeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ProductSubtractorNodeViewModel : ProcessNodeViewModel
    {
        private ProcessModel _processModel;
        private ProductSubtractorNode _subtractorModel;

        public ProductSubtractorNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<NodeOutputSpec>();
        }

        public override string NodeTypeName => "Product Subtractor Node";

        public ObservableCollection<NodeOutputSpec> AvailableNodeOutputSpecs { get; }

        private NodeOutputSpec _selectedNodeToDeduct;
        public NodeOutputSpec SelectedNodeToDeduct
        {
            get { return _selectedNodeToDeduct; }
            set
            {
                SetField(ref _selectedNodeToDeduct, value);
                if (_selectedNodeToDeduct != null) _subtractorModel.PrecedingNodeId = _selectedNodeToDeduct.Id;
            }
        }

        private DataFunctionViewModel _deductionFunctionViewModel;
        public DataFunctionViewModel DeductionFunctionViewModel
        {
            get { return _deductionFunctionViewModel; }
            set { SetField(ref _deductionFunctionViewModel, value); }
        }

        public void Load(ProcessNode deducterNode, ProcessModel processModel)
        {
            base.Load(deducterNode);
            _subtractorModel = (ProductSubtractorNode)deducterNode;
            _processModel = processModel;

            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in _processModel.GetAllNodeSpecs().Where(x => x.Id > 0 && x.Id != _subtractorModel.Id))
            {
                AvailableNodeOutputSpecs.Add(spec);
            }
            SelectedNodeToDeduct = AvailableNodeOutputSpecs.FirstOrDefault(x => x.Id == _subtractorModel.PrecedingNodeId);

            DataFunctionViewModel vm = this.Resolve<DataFunctionViewModel>();
            vm.Load(_subtractorModel.ProductDeductionFunction, null);
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DataFunctionViewModel.SelectedUnitForm))
                {
                    _subtractorModel.ProductDeductionFunction = vm.GetBackingModel();
                }
            };
            DeductionFunctionViewModel = vm;
        }
    }
}

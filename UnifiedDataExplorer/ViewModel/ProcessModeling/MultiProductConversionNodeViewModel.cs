using DotNetCommon.MVVM;
using EmissionsMonitorModel.ProcessModeling;
using System.Collections.ObjectModel;
using System.Linq;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class MultiProductConversionNodeViewModel : ProcessNodeViewModel
    {
        private ProcessModel _processModel;
        private MultiProductConversionNode _multiConversionNode;

        public MultiProductConversionNodeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AvailableNodeOutputSpecs = new ObservableCollection<CheckableItemViewModel>();
        }

        public override string NodeTypeName => "Multi Product Conversion Node";

        public ObservableCollection<CheckableItemViewModel> AvailableNodeOutputSpecs { get; }

        private DataFunctionViewModel _newProductFunctionViewModel;
        public DataFunctionViewModel NewProductFunctionViewModel
        {
            get { return _newProductFunctionViewModel; }
            set { SetField(ref _newProductFunctionViewModel, value); }
        }

        public void Load(ProcessNode processNode, ProcessModel model)
        {
            base.Load(processNode);
            _multiConversionNode = processNode as MultiProductConversionNode;
            _processModel = model;

            AvailableNodeOutputSpecs.Clear();
            foreach (var spec in _processModel.GetAllNodeSpecs().Where(x => x.Id > 0 && x.Id != _multiConversionNode.Id))
            {
                var vm = new CheckableItemViewModel
                {
                    IsChecked = _multiConversionNode.PrecedingNodeIds.Contains(spec.Id),
                    Item = spec
                };
                vm.PropertyChanged += OnCheckableItemChanged;
                AvailableNodeOutputSpecs.Add(vm);
            }

            DataFunctionViewModel dfvm = this.Resolve<DataFunctionViewModel>();
            dfvm.Load(_multiConversionNode.NewProductFunction, null);
            dfvm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(DataFunctionViewModel.SelectedUnitForm))
                {
                    _multiConversionNode.NewProductFunction = dfvm.GetBackingModel();
                }
            };
            NewProductFunctionViewModel = dfvm;
        }

        private void OnCheckableItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CheckableItemViewModel.IsChecked))
            {
                CheckableItemViewModel vm = sender as CheckableItemViewModel;
                if (vm.IsChecked && !_multiConversionNode.PrecedingNodeIds.Contains(vm.ItemId)) _multiConversionNode.PrecedingNodeIds.Add(vm.ItemId);
                else if (!vm.IsChecked && _multiConversionNode.PrecedingNodeIds.Contains(vm.ItemId)) _multiConversionNode.PrecedingNodeIds.Remove(vm.ItemId);
            }
        }
    }
}

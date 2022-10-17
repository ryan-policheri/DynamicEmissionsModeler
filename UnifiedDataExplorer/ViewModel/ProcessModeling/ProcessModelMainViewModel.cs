using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.VirtualFileSystem;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ProcessModelMainViewModel : RobustViewModelBase
    {
        private readonly IVirtualFileSystemRepository _repo;
        private ModelSaveItem _saveItem;
        private ProcessModel _model;

        public ProcessModelMainViewModel(IVirtualFileSystemRepository repo, NodesNavigationViewModel nodeNavViewModel, RobustViewModelDependencies facade) : base(facade)
        {
            _repo = repo;
            NodesNavigationViewModel = nodeNavViewModel;
            NodesNavigationViewModel.PropertyChanged += NodesNavigationViewModel_PropertyChanged;
            SaveCommand = new DelegateCommand<bool?>(OnSave);
        }

        public async Task LoadAsync(int id = -1)
        {
            if (id > 0)
            {
                _saveItem = await _repo.GetModelSaveItemAsync(id);
                _model = _saveItem.ProcessModelJsonDetails.ConvertJsonToObject<ProcessModel>();
            }
            else _model = new ProcessModel();

            NodesNavigationViewModel.Load(_model);
        }

        public NodesNavigationViewModel NodesNavigationViewModel { get; }

        public ProcessNodeViewModel SelectedProcessNode { get; private set; }

        public ICommand SaveCommand { get; }

        private void NodesNavigationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(NodesNavigationViewModel.SelectedProcessNode))
            {
                SelectedProcessNode = NodesNavigationViewModel.SelectedProcessNode; OnPropertyChanged(nameof(SelectedProcessNode));
            }
        }

        private async void OnSave(bool? saveAs)
        {
            if (_saveItem == null || saveAs.Value)
            {
                var vm = this.Resolve<EnergyModelFileSystemViewModel>();
                _saveItem = new ModelSaveItem { ProcessModelJsonDetails = _model.ToJson() };
                vm.SaveData = _saveItem;
                await vm.LoadAsync(FileSystemMode.SaveOrManage);
                this.DialogService.ShowModalWindow(vm);
            }
            else
            {
                _saveItem.ProcessModelJsonDetails = _model.ToJson();
                await _repo.SaveModelSaveItemAsync(_saveItem);
            }
        }
    }
}

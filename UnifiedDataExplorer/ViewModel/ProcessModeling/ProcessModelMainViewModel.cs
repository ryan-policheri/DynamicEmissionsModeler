using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.VirtualFileSystem;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ProcessModelMainViewModel : RobustViewModelBase
    {
        private readonly ProcessNodesViewModel _processNodesVm;
        private readonly IVirtualFileSystemRepository _repo;
        private ModelSaveItem _saveItem;
        private ProcessModel _model;

        public ProcessModelMainViewModel(ProcessNodesViewModel processNodesVm, IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            _processNodesVm = processNodesVm;
            _repo = repo;
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

            _processNodesVm.Load(_model);
            CurrentChild = _processNodesVm;
        }

        public ICommand SaveCommand { get; }

        public ViewModelBase CurrentChild { get; private set; }

        private async void OnSave(bool? saveAs)
        {
            if (_saveItem == null || saveAs.Value)
            {
                var vm = this.Resolve<EnergyModelFileSystemViewModel>();
                _saveItem = new ModelSaveItem { ProcessModelJsonDetails = _model.ToJson<ProcessModel>() };
                vm.SaveData = _saveItem;
                await vm.LoadAsync(FileSystemMode.SaveOrManage);
                this.DialogService.ShowModalWindow(vm);
            }
            else
            {
                _saveItem.ProcessModelJsonDetails = _model.ToJson<ProcessModel>();
                _saveItem = await _repo.SaveModelSaveItemAsync(_saveItem);
            }
        }
    }
}

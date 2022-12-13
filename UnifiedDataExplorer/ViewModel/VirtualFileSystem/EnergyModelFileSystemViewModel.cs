using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using System.Threading.Tasks;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.VirtualFileSystem
{
    public class EnergyModelFileSystemViewModel : VirtualFileSystemViewModel
    {
        public EnergyModelFileSystemViewModel(IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) : base(repo, facade)
        {
        }

        public async Task LoadAsync(FileSystemMode mode)
        {
            await base.LoadAsync(SystemRoots.ENERGY_MODELS, mode);
        }

        protected override async Task<SaveItem> OnSaveAsync(SaveItem saveItem)
        {
            var modelSave = (ModelSaveItem)saveItem;
            if (modelSave != null && this.SelectedFolder != null)
            {
                modelSave = await Repo.SaveModelSaveItemAsync(modelSave);
                return modelSave;
            }
            return null;
        }

        protected override async Task OnOpenSaveItemAsync(int id)
        {
            ProcessModelMainViewModel viewModel = this.Resolve<ProcessModelMainViewModel>();
            await viewModel.LoadAsync(id);
            this.DialogService.OpenSecondaryWindow<ProcessModelMainViewModel>(viewModel);
        }
    }
}

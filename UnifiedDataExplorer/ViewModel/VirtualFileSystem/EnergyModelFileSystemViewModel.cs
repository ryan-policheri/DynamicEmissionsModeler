using System;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using System.Threading.Tasks;
using UnifiedDataExplorer.ViewModel.Base;

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
            throw new NotImplementedException();
        }

        protected override async Task OnOpenSaveItemAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

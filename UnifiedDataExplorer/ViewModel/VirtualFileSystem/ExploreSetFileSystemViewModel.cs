using System.Threading.Tasks;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.VirtualFileSystem
{
    public class ExploreSetFileSystemViewModel : VirtualFileSystemViewModel
    {
        public ExploreSetFileSystemViewModel(IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) : base(repo, facade)
        {
        }

        public async Task LoadAsync(FileSystemMode mode)
        {
            await base.LoadAsync(SystemRoots.EXPLORE_SETS, mode);
        }

        protected override async Task<SaveItem> OnSaveAsync(SaveItem saveItem)
        {
            var exploreSet = (ExploreSetSaveItem)saveItem;
            if (exploreSet != null && this.SelectedFolder != null)
            {
                exploreSet = await Repo.CreateExploreSetItemAsync(exploreSet);
                return exploreSet;
            }
            return null;
        }

        protected override async Task OnOpenSaveItemAsync(int id)
        {
            ExploreSetSaveItem exploreSet = await Repo.GetExploreSetItemAsync(id); 
            MessageHub.Publish(new OpenSaveItemEvent
            {
                Sender = this,
                SenderTypeName = nameof(ExploreSetFileSystemViewModel),
                SaveItem = exploreSet
            });
        }
    }
}

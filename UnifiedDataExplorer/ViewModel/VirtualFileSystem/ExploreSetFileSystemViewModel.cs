using System;
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

        public ExploreSetSaveItem SaveItem { get; set; }

        public async Task LoadAsync(FileSystemMode mode)
        {
            await base.LoadAsync(SystemRoots.EXPLORE_SETS, mode);
        }

        public override async void OnSaveAsync(SaveItem saveItem)
        {
            var exploreSet = (ExploreSetSaveItem)saveItem;
            if (exploreSet != null && this.SelectedFolder != null)
            {
                exploreSet = await Repo.CreateExploreSetItemAsync(exploreSet);
                await LoadAsync(Mode);
            }
        }

        public override async void OnOpenSaveItemAsync()
        {
            var exploreSet = await Repo.GetExploreSetItemAsync(this.SelectedSaveItem.SaveItemId); 
            MessageHub.Publish(new OpenSaveItemEvent
            {
                Sender = this,
                SenderTypeName = nameof(ExploreSetFileSystemViewModel),
                SaveItem = exploreSet
            });
        }
    }
}

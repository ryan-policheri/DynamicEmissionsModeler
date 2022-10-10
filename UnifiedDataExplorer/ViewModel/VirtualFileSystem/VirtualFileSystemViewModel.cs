using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.VirtualFileSystem
{
    public abstract class VirtualFileSystemViewModel : RobustViewModelBase
    {
        protected FileSystemMode Mode;
        protected readonly IVirtualFileSystemRepository Repo;

        public VirtualFileSystemViewModel(IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) :
            base(facade)
        {
            Repo = repo;
            Folders = new ObservableCollection<FolderSaveItemViewModel>();
        }

        public Folder SelectedFolder { get; set; }

        public ObservableCollection<FolderSaveItemViewModel> Folders { get; }

        public SaveItem SelectedSaveItem { get; set; }

        public async Task LoadAsync(string rootFolderName, FileSystemMode mode)
        {
            Mode = mode;

            var roots = await Repo.GetAllRootFoldersAsync();
            var exploreSetRoot = roots.OrderBy(x => x.FolderId).FirstOrDefault(x => x.FolderName == rootFolderName);
            if (exploreSetRoot == null) exploreSetRoot = await Repo.CreateRootFolderAsync(new Folder { FolderName = rootFolderName });

            var root = await Repo.GetFolderRecursiveAsync(exploreSetRoot.FolderId);
            FolderSaveItemViewModel vm = new FolderSaveItemViewModel(root);
            vm.IsExpanded = true;
            Folders.Clear();
            Folders.Add(vm);
        }

        public abstract void OnSaveAsync(SaveItem saveItem);

        public abstract void OnOpenSaveItemAsync();

        public virtual async void OnDeleteSaveItem()
        {
            if (this.SelectedSaveItem != null)
            {
                _ = await Repo.DeleteFolderAsync(this.SelectedSaveItem.SaveItemId);
            }
        }
    }

    public enum FileSystemMode
    {
        OpenOnly,
        SaveOrManage,
        OpenOrManage
    }
}
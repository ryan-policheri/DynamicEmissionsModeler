using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.VirtualFileSystem
{
    public abstract class VirtualFileSystemViewModel : RobustViewModelBase
    {
        protected FileSystemMode Mode;
        protected readonly IVirtualFileSystemRepository Repo;

        public VirtualFileSystemViewModel(IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            Repo = repo;
            DeleteCommand = new DelegateCommand<FolderSaveItemViewModel>(OnDelete);
            Folders = new ObservableCollection<FolderSaveItemViewModel>();
        }

        public ICommand DeleteCommand { get; }


        private FolderSaveItemViewModel _selectedFolder;
        public FolderSaveItemViewModel SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                if (value.ElementType == FolderOrSaveItem.Folder)
                {
                    SetField(ref _selectedFolder, value);
                }
            }
        }

        public ObservableCollection<FolderSaveItemViewModel> Folders { get; }

        public SaveItem SelectedSaveItem { get; set; }

        public bool CanSave => this.Mode == FileSystemMode.SaveOrManage;
        public string SaveItemName { get; set; }

        public bool CanManage => this.Mode == FileSystemMode.SaveOrManage || this.Mode == FileSystemMode.OpenOrManage;

        protected async Task LoadAsync(string rootFolderName, FileSystemMode mode)
        {
            Mode = mode;

            var roots = await Repo.GetAllRootFoldersAsync();
            var exploreSetRoot = roots.OrderBy(x => x.FolderId).FirstOrDefault(x => x.FolderName == rootFolderName);
            if (exploreSetRoot == null) exploreSetRoot = await Repo.CreateRootFolderAsync(new Folder { FolderName = rootFolderName });

            var root = await Repo.GetFolderRecursiveAsync(exploreSetRoot.FolderId);
            FolderSaveItemViewModel vm = new FolderSaveItemViewModel(root, null);
            vm.IsExpanded = true;
            Folders.Clear();
            Folders.Add(vm);
        }

        public abstract void OnSaveAsync(SaveItem saveItem);

        public abstract void OnOpenSaveItemAsync();

        public async Task<Folder> SaveFolder(Folder folder)
        {
            return await Repo.SaveFolderAsync(folder);
        }

        public async Task<SaveItem> UpdateDirectoryInfo(SaveItem item)
        {
            return await Repo.SaveSaveItemInfo(item);
        }

        private async void OnDelete(FolderSaveItemViewModel item)
        {
            switch (item.ElementType)
            {
                case FolderOrSaveItem.Folder:
                    await Repo.DeleteFolderAsync((item.GetBackingModel() as Folder).FolderId);
                    item.Parent.RemoveChild(item);
                    break;
                default:
                    await Repo.DeleteSaveItemAsync((item.GetBackingModel() as SaveItem).SaveItemId);
                    item.Parent.RemoveChild(item);
                    break;
            }
        }

        public async void MoveItemToFolder(FolderSaveItemViewModel source, FolderSaveItemViewModel target)
        {
            if (source.ElementType != FolderOrSaveItem.Folder && target.ElementType == FolderOrSaveItem.Folder)
            {
                SaveItem saveItem = source.GetBackingModel() as SaveItem;
                Folder folder = target.GetBackingModel() as Folder;
                if (saveItem.FolderId != folder.FolderId)
                {
                    saveItem.FolderId = folder.FolderId;
                    await Repo.SaveSaveItemInfo(saveItem);
                    source.Parent.RemoveChild(source);
                    target.AddChild(source);
                }
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
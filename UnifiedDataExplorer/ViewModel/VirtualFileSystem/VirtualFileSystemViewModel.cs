using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.VirtualFileSystem
{
    public abstract class VirtualFileSystemViewModel : RobustViewModelBase
    {
        private string _rootFolderName;
        protected FileSystemMode Mode;
        protected readonly IVirtualFileSystemRepository Repo;

        public VirtualFileSystemViewModel(IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            Repo = repo;
            Folders = new ObservableCollection<FolderSaveItemViewModel>();
            OpenCommand = new DelegateCommand<FolderSaveItemViewModel>(OnOpenCommand);
            SaveCommand = new DelegateCommand(OnSave);
            DeleteCommand = new DelegateCommand<FolderSaveItemViewModel>(OnDelete);
            CancelCommand = new DelegateCommand(CloseViewModel);
            this.MessageHub.Subscribe<SaveViewModelEvent>(OnViewModelSavedEvent);
        }

        //Commands
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }


        //Directory View
        private FolderSaveItemViewModel _selectedFolder;
        public FolderSaveItemViewModel SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                if (value != null && value.ElementType == FolderOrSaveItem.Folder)
                {
                    SetField(ref _selectedFolder, value);
                    OnPropertyChanged(nameof(SaveReady));
                }
            }
        }

        public ObservableCollection<FolderSaveItemViewModel> Folders { get; }

        //Load
        protected async Task LoadAsync(string rootFolderName, FileSystemMode mode)
        {
            _rootFolderName = rootFolderName;
            Mode = mode;

            var roots = await Repo.GetAllRootFoldersAsync();
            var root = roots.OrderBy(x => x.FolderId).FirstOrDefault(x => x.FolderName == _rootFolderName);
            if (root == null) root = await Repo.CreateRootFolderAsync(new Folder { FolderName = _rootFolderName });

            var rootFolder = await Repo.GetFolderRecursiveAsync(root.FolderId);
            FolderSaveItemViewModel vm = new FolderSaveItemViewModel(rootFolder, null);
            vm.IsExpanded = true;
            Folders.Clear();
            Folders.Add(vm);
        }

        //Directory Management
        public bool CanManage => this.Mode == FileSystemMode.SaveOrManage || this.Mode == FileSystemMode.OpenOrManage;

        public async Task<Folder> SaveFolder(Folder folder)
        {
            return await Repo.SaveFolderAsync(folder);
        }

        public async Task<SaveItem> UpdateDirectoryInfo(SaveItem item)
        {
            return await Repo.SaveSaveItemInfo(item);
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


        //Saving: requires child implementation that knows how to save the data.
        public bool CanSave => this.Mode == FileSystemMode.SaveOrManage;
        public bool SaveReady => this.SelectedFolder != null && this.SaveData?.SaveItemName != null;
        public SaveItem SaveData { get; set; }

        public string SaveItemName
        {
            get { return SaveData?.SaveItemName; }
            set
            {
                if (SaveData != null)
                {
                    SaveData.SaveItemName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SaveReady));
                }
            }
        }
        protected abstract Task<SaveItem> OnSaveAsync(SaveItem saveItem);

        private async void OnSave()
        {
            this.SaveData.FolderId = (this.SelectedFolder.GetBackingModel() as Folder).FolderId;
            SaveItem item = await OnSaveAsync(this.SaveData);
            CloseViewModel();
            MessageHub.Publish(new SaveViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(VirtualFileSystemViewModel)
            });
        }

        private async void OnViewModelSavedEvent(SaveViewModelEvent args)
        {
            if (args.SenderTypeName == nameof(VirtualFileSystemViewModel) && args.Sender != null)
            {
                await this.LoadAsync(_rootFolderName, Mode);
            }
        }

        //Opening: requires child implementation that know how to retrieve the data
        protected abstract Task OnOpenSaveItemAsync(int id);

        private async void OnOpenCommand(FolderSaveItemViewModel sender)
        {
            SaveItem item = sender?.GetBackingModel() as SaveItem;
            if (item != null)
            {
                await OnOpenSaveItemAsync(item.SaveItemId);
            }
        }


        //Cancel/closing
        private void CloseViewModel()
        {
            MessageHub.Publish(new CloseViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(VirtualFileSystemViewModel)
            });
        }

    }

    public enum FileSystemMode
    {
        OpenOnly,
        SaveOrManage,
        OpenOrManage
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.VirtualFileSystem
{
    public class FolderSaveItemViewModel : ViewModelBase
    {
        private object _backingModel;

        public FolderSaveItemViewModel(Folder folder, FolderSaveItemViewModel parent)
        {
            Parent = parent;

            var tempHoldChildFolders = folder.ChildFolders; folder.ChildFolders = null;
            var tempHoldSaveItems = folder.SaveItems; folder.SaveItems = null;
            _backingModel = folder.Copy();
            folder.ChildFolders = tempHoldChildFolders;
            folder.SaveItems = tempHoldSaveItems;

            ElementType = FolderOrSaveItem.Folder;
            DisplayText = folder.FolderName;
            Children = new ObservableCollection<FolderSaveItemViewModel>();
            ToggleRename = new DelegateCommand(() => { this.CanRename = !this.CanRename; });

            if (folder.ChildFolders != null)
            {
                foreach (Folder childFolder in folder.ChildFolders.OrderBy(x => x.FolderName))
                {
                    Children.Add(new FolderSaveItemViewModel(childFolder, this));
                }
            }

            if (folder.SaveItems != null)
            {
                foreach (SaveItem saveItem in folder.SaveItems.OrderBy(x => x.SaveItemName))
                {
                    Children.Add(new FolderSaveItemViewModel(saveItem, this));
                }
            }

            this.PropertyChanged += (sender, args) => { if(args.PropertyName == nameof(this.Children)) OnPropertyChanged(nameof(CanDelete)); };
        }

        public FolderSaveItemViewModel(SaveItem saveItem, FolderSaveItemViewModel parent)
        {
            Parent = parent;
            _backingModel = saveItem;
            ElementType = (FolderOrSaveItem)saveItem.SaveItemType;
            Children = new ObservableCollection<FolderSaveItemViewModel>();
            DisplayText = saveItem.SaveItemName;
            ToggleRename = new DelegateCommand(() => { this.CanRename = !this.CanRename; });
        }

        public ICommand ToggleRename { get; }

        public bool CanExpand => this.Children.Count > 0;

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded;}
            set { SetField(ref _isExpanded, value); }
        }

        private bool _canRename;
        public bool CanRename { get{ return _canRename;} set { SetField(ref _canRename, value); OnPropertyChanged(nameof(IsReadOnly)); } }
        public bool IsReadOnly => !CanRename;

        public bool CanDelete => Parent != null && ((ElementType != FolderOrSaveItem.Folder) || (this.Children.Count == 0));

        private string _displayText;

        public string DisplayText
        {
            get { return _displayText; }
            set { SetField(ref _displayText, value); }
        }

        public FolderOrSaveItem ElementType { get; set; }

        public FolderSaveItemViewModel Parent { get; private set; }

        public ObservableCollection<FolderSaveItemViewModel> Children { get; }

        public object GetBackingModel() => _backingModel;

        public void AddChild(FolderSaveItemViewModel child)
        {
            Children.Add(child);
            OnPropertyChanged(nameof(CanDelete));
        }

        public void RemoveChild(FolderSaveItemViewModel child)
        {
            Children.Remove(child);
            OnPropertyChanged(nameof(CanDelete));
        }
    }

    public enum FolderOrSaveItem
    {
        Folder = -1,
        SaveItem = SaveItemTypes.SaveItem,
        ExploreSetSaveItem = SaveItemTypes.ExploreSetSaveItem,
        ModelSaveItem = SaveItemTypes.ModelSaveItem
    }
}

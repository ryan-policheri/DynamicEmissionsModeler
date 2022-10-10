using System.Collections.Generic;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.VirtualFileSystem
{
    public class FolderSaveItemViewModel : ViewModelBase
    {
        private object _backingModel;

        public FolderSaveItemViewModel(Folder folder)
        {
            var tempHoldChildFolders = folder.ChildFolders; folder.ChildFolders = null;
            var tempHoldSaveItems = folder.SaveItems; folder.SaveItems = null;
            _backingModel = folder.Copy();
            folder.ChildFolders = tempHoldChildFolders;
            folder.SaveItems = tempHoldSaveItems;

            ElementType = FolderOrSaveItem.Folder;
            DisplayText = folder.FolderName;
            Children = new List<FolderSaveItemViewModel>();

            if (folder.SaveItems != null)
            {
                foreach (SaveItem saveItem in folder.SaveItems)
                {
                    Children.Add(new FolderSaveItemViewModel(saveItem));
                }
            }

            if (folder.ChildFolders != null)
            {
                foreach (Folder childFolder in folder.ChildFolders)
                {
                    Children.Add(new FolderSaveItemViewModel(childFolder));
                }
            }
        }

        public FolderSaveItemViewModel(SaveItem saveItem)
        {
            _backingModel = saveItem;
            ElementType = (FolderOrSaveItem)saveItem.SaveItemType;
            Children = new List<FolderSaveItemViewModel>();
            DisplayText = saveItem.SaveItemName;
        }

        public bool CanExpand => this.Children.Count > 0;

        public bool IsExpanded { get; set; }

        public string DisplayText { get; set; }

        public FolderOrSaveItem ElementType { get; set; }

        public List<FolderSaveItemViewModel> Children { get; }
    }

    public enum FolderOrSaveItem
    {
        Folder = -1,
        SaveItem = SaveItemTypes.SaveItem,
        ExploreSetSaveItem = SaveItemTypes.ExploreSetSaveItem,
        ModelSaveItem = SaveItemTypes.ModelSaveItem
    }
}

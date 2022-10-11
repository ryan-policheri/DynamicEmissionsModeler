using System;
using System.Windows;
using System.Windows.Controls;
using EmissionsMonitorModel.VirtualFileSystem;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.View.VirtualFileSystem
{
    public partial class VirtualFileSystemView : UserControl
    {
        private VirtualFileSystemViewModel _viewModel => this.DataContext as VirtualFileSystemViewModel;

        public VirtualFileSystemView()
        {
            InitializeComponent();
        }

        private void FolderView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView != null)
            {
                _viewModel.SelectedFolder = treeView.SelectedItem as FolderSaveItemViewModel;
            }
        }

        private async void FolderRenameBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box != null)
            {
                FolderSaveItemViewModel senderVm = box.DataContext as FolderSaveItemViewModel;
                if (senderVm != null && senderVm.CanRename && !String.IsNullOrWhiteSpace(box.Text))
                {
                    Folder folder = senderVm.GetBackingModel() as Folder;
                    folder.FolderName = box.Text;
                    await _viewModel.SaveFolder(folder);
                    senderVm.DisplayText = box.Text;
                    senderVm.ToggleRename.Execute(null);
                }
            }
        }

        private async void OnAddChildFolderClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                FolderSaveItemViewModel senderVm = item.DataContext as FolderSaveItemViewModel;
                if (senderVm != null)
                {
                    Folder parent = senderVm.GetBackingModel() as Folder;
                    Folder child = new Folder { ParentFolderId = parent.ParentFolderId, FolderName = "New Folder" };
                    await _viewModel.SaveFolder(child);
                    senderVm.Children.Add(new FolderSaveItemViewModel(child, senderVm));
                }
            }
        }

        private async void FileRenameBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box != null)
            {
                FolderSaveItemViewModel senderVm = box.DataContext as FolderSaveItemViewModel;
                if (senderVm != null && senderVm.CanRename && !String.IsNullOrWhiteSpace(box.Text))
                {
                    SaveItem item = senderVm.GetBackingModel() as SaveItem;
                    item.SaveItemName = box.Text;
                    await _viewModel.UpdateDirectoryInfo(item);
                    senderVm.DisplayText = box.Text;
                    senderVm.ToggleRename.Execute(null);
                }
            }
        }
    }
}

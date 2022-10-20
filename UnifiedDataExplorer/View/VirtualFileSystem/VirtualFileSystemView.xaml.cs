using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EmissionsMonitorModel.VirtualFileSystem;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.View.VirtualFileSystem
{
    public partial class VirtualFileSystemView : UserControl
    {
        public VirtualFileSystemViewModel ViewModel => this.DataContext as VirtualFileSystemViewModel;

        public VirtualFileSystemView()
        {
            InitializeComponent(); 
        }

        //For saving
        private void FolderView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView != null && ViewModel != null)
            {
                ViewModel.SelectedFolder = treeView.SelectedItem as FolderSaveItemViewModel;
            }
        }

        //For managing
        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!ViewModel.CanManage) e.Handled = true;
        }

        private void OnRenameClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (sender as MenuItem);
            (item.DataContext as FolderSaveItemViewModel)?.ToggleRename.Execute(null);

            var owner = ((ContextMenu)item?.Parent)?.PlacementTarget as StackPanel;
            TextBox box = owner?.Children.OfType<TextBox>().FirstOrDefault();
            if (box != null)
            {
                box.Focus();
                box.SelectAll();
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
                    await ViewModel.SaveFolder(folder);
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
                    Folder child = new Folder { ParentFolderId = parent.FolderId, FolderName = "New Folder" };
                    Folder savedFolder = await ViewModel.SaveFolder(child);
                    senderVm.AddChild(new FolderSaveItemViewModel(savedFolder, senderVm));
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
                    await ViewModel.UpdateDirectoryInfo(item);
                    senderVm.DisplayText = box.Text;
                    senderVm.ToggleRename.Execute(null);
                }
            }
        }

        private void OnFileMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StackPanel element = sender as StackPanel;
                if (element != null)
                {
                    DataObject obj = new DataObject(element.DataContext);
                    DragDrop.DoDragDrop(element, obj, DragDropEffects.Move);
                }
            }
        }

        private void OnFolderDragOver(object sender, DragEventArgs e)
        {
            StackPanel element = sender as StackPanel;
            if (element != null) element.Background = new SolidColorBrush(Colors.Gray);
        }

        private void OnFolderDrop(object sender, DragEventArgs e)
        {
            StackPanel element = sender as StackPanel;
            FolderSaveItemViewModel source = e.Data.GetData(typeof(FolderSaveItemViewModel)) as FolderSaveItemViewModel;
            FolderSaveItemViewModel target = element?.DataContext as FolderSaveItemViewModel;

            if (source != null && target != null)
            {
                ViewModel.MoveItemToFolder(source, target);
            }
            if (element != null) element.Background = new SolidColorBrush(Colors.White);
        }

        private void OnFolderDragLeave(object sender, DragEventArgs e)
        {
            StackPanel element = sender as StackPanel;
            if (element != null) element.Background = new SolidColorBrush(Colors.White);
        }

        private void OnOpenItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                FolderSaveItemViewModel senderVm = (sender as StackPanel)?.DataContext as FolderSaveItemViewModel;
                ViewModel.OpenCommand.Execute(senderVm);
            }
        }
    }
}

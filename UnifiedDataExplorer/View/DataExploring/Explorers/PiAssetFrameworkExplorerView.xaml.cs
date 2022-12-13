using DotNetCommon.MVVM;
using System.Windows;
using System.Windows.Controls;
using UnifiedDataExplorer.ModelWrappers;
using UnifiedDataExplorer.ViewModel.DataExploring.Explorers;

namespace UnifiedDataExplorer.View.DataExploring.Explorers
{
    public partial class PiAssetFrameworkExplorerView : UserControl
    {
        private PiAssetFrameworkExplorerViewModel _viewModel => this.DataContext as PiAssetFrameworkExplorerViewModel;

        public PiAssetFrameworkExplorerView()
        {
            InitializeComponent();
        }

        private async void LazyLoadedTree_Expanded(object sender, RoutedEventArgs args)
        {
            if (sender != null && args != null && args.OriginalSource != null && args.OriginalSource is TreeViewItem)
            {
                TreeViewItem treeItem = args.OriginalSource as TreeViewItem;
                if (treeItem != null)
                {
                    LazyTreeItemViewModel treeItemViewModel = treeItem.DataContext as LazyTreeItemViewModel;
                    if (treeItemViewModel != null && !treeItemViewModel.IsExpanded && !treeItemViewModel.ChildrenLoaded)
                    {
                        await _viewModel.LoadChildrenAsync(treeItemViewModel);
                        treeItemViewModel.IsExpanded = true;
                        args.Handled = true;
                    }
                    else if (treeItemViewModel != null && treeItemViewModel.IsLeaf)
                    {
                        await _viewModel.PeformLeafActionAsync(treeItemViewModel);
                        args.Handled = true;
                    }
                }
            }
        }

        private void RenderValuesButton_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button?.DataContext != null)
            {
                LazyTreeItemViewModel vm = (LazyTreeItemViewModel)button.DataContext;
                if(vm != null)
                {
                    ServerDatabaseAssetWrapper model = vm.GetBackingModel() as ServerDatabaseAssetWrapper;
                    if(model != null)
                    {
                        if(model.IsAsset()) button.Visibility = Visibility.Visible;
                    }    
                }
            }
        }
    }
}

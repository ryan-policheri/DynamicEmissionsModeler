using System.Windows;
using System.Windows.Controls;
using DotNetCommon.MVVM;
using UnifiedDataExplorer.ViewModel;

namespace UnifiedDataExplorer.View
{
    public partial class EiaDatasetFinderView : UserControl
    {
        private EiaDatasetFinderViewModel _viewModel => this.DataContext as EiaDatasetFinderViewModel;

        public EiaDatasetFinderView()
        {
            InitializeComponent();
        }

        private async void LazyLoadedTree_Expanded(object sender, RoutedEventArgs args)
        {
            if(sender != null && args != null && args.OriginalSource != null && args.OriginalSource is TreeViewItem)
            {
                TreeViewItem treeItem = args.OriginalSource as TreeViewItem;
                if(treeItem != null)
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
                        _viewModel.PeformLeafActionAsync(treeItemViewModel);
                        args.Handled = true;
                    }
                }
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using UnifiedDataExplorer.ViewModel.ProcessModeling;

namespace UnifiedDataExplorer.View.ProcessModeling
{
    public partial class NodesNavigationView : UserControl
    {
        public NodesNavigationViewModel ViewModel => (DataContext as NodesNavigationViewModel);

        public NodesNavigationView()
        {
            InitializeComponent();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ProcessNodeViewModel senderVm = (sender as MenuItem)?.DataContext as ProcessNodeViewModel;
            if (senderVm != null) { ViewModel.RemoveProcessNode(senderVm); }
        }
    }
}

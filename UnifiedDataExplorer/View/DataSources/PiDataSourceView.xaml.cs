using System.Windows;
using System.Windows.Controls;
using UnifiedDataExplorer.ViewModel.DataSources;

namespace UnifiedDataExplorer.View.DataSources
{
    public partial class PiDataSourceView : UserControl
    {
        public PiDataSourceView()
        {
            InitializeComponent();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs args)
        {
            if (this.DataContext != null) ((PiDataSourceViewModel)this.DataContext).PiPassword = ((PasswordBox)sender).Password;
        }

        private void PiDataSourceView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null) (Passbox).Password = ((PiDataSourceViewModel)this.DataContext).PiPassword;
        }
    }
}

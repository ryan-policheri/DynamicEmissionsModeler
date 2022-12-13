using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.ViewModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.View.ProcessModeling
{
    public partial class FunctionFactorView : UserControl
    {
        public FunctionFactorViewModel ViewModel => DataContext as FunctionFactorViewModel;

        public FunctionFactorView()
        {
            InitializeComponent();
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            Border element = sender as Border;
            if (element != null) element.Background = new SolidColorBrush(Colors.Gray);
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            Border element = sender as Border;
            if (element != null) element.Background = new SolidColorBrush(Colors.White);
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            Border element = sender as Border;
            DataSourceSeriesUri source = e.Data.GetData(typeof(DataSourceSeriesUri)) as DataSourceSeriesUri;

            if (source != null)
            {
                ViewModel.SetSeries(source);
            }
            if (element != null) element.Background = new SolidColorBrush(Colors.White);
        }
    }
}

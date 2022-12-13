using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UnifiedDataExplorer.ViewModel.ProcessModeling;

namespace UnifiedDataExplorer.View.ProcessModeling
{
    public partial class ExecutionView : UserControl
    {
        private ExecutionViewModel ViewModel => this.DataContext as ExecutionViewModel;

        public ExecutionView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContextChanged += DataPointsView_DataContextChanged;
            OnDataChanged();
        }

        private void DataPointsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnDataChanged();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExecutionViewModel.SelectedStreamResults) && ViewModel != null)
            {
                OnDataChanged();
            }
        }

        private void OnDataChanged()
        {
            if (ViewModel?.SelectedStreamResults != null)
            {
                MainDataGrid.Visibility = Visibility.Visible;
                CollectionViewSource collection = new CollectionViewSource();
                collection.Source = ViewModel.SelectedStreamResults;
                MainDataGrid.ItemsSource = collection.View;
            }
        }
    }
}

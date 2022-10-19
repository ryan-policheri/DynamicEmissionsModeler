using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints;

namespace UnifiedDataExplorer.View.DataExploring.ExplorePoints
{
    public partial class TimeSeriesExplorePointView : UserControl
    {
        private TimeSeriesExplorePointViewModel ViewModel => this.DataContext as TimeSeriesExplorePointViewModel;

        public TimeSeriesExplorePointView()
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
            if (e.PropertyName == nameof(TimeSeriesExplorePointViewModel.DataSet) && ViewModel != null)
            {
                OnDataChanged();
            }
        }

        private void OnDataChanged()
        {
            if (ViewModel?.DataSet != null)
            {
                MainDataGrid.Visibility = Visibility.Visible;
                CollectionViewSource collection = new CollectionViewSource();
                collection.Source = ViewModel.DataSet;
                MainDataGrid.ItemsSource = collection.View;
            }
        }
    }
}

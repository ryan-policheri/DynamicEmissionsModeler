using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints;

namespace UnifiedDataExplorer.View.DataExploring.ExplorePoints
{
    public partial class EiaSeriesExplorePointView : UserControl
    {
        private EiaSeriesExplorePointViewModel ViewModel => this.DataContext as EiaSeriesExplorePointViewModel;

        public EiaSeriesExplorePointView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContextChanged += SeriesView_DataContextChanged;
            OnDataChanged();
        }

        private void SeriesView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnDataChanged();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(EiaSeriesExplorePointViewModel.DataSet) && ViewModel != null)
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints;

namespace UnifiedDataExplorer.View.DataExploring.ExplorePoints
{
    public partial class PiInterpolatedDataExplorePointView : UserControl
    {
        private PiInterpolatedDataExplorePointViewModel ViewModel => this.DataContext as PiInterpolatedDataExplorePointViewModel;

        public PiInterpolatedDataExplorePointView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContextChanged += PiDataPointsView_DataContextChanged;
            OnDataChanged();
        }

        private void PiDataPointsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnDataChanged();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PiInterpolatedDataExplorePointViewModel.DataSet) && ViewModel != null)
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
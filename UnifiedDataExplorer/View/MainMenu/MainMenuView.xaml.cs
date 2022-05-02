using System.Windows.Controls;
using UnifiedDataExplorer.ViewModel.MainMenu;

namespace UnifiedDataExplorer.View.MainMenu
{
    public partial class MainMenuView : UserControl
    {
        public MainMenuViewModel ViewModel => (MainMenuViewModel)DataContext;

        public MainMenuView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(MainMenuViewModel.IsLoading))
            {
                if(ViewModel.IsLoading) LoadingIcon.Visibility = System.Windows.Visibility.Visible;
                else LoadingIcon.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}

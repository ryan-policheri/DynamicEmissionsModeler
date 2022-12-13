using System.Windows;
using UnifiedDataExplorer.ViewModel;

namespace UnifiedDataExplorer
{
    public partial class SecondaryWindow : Window
    {
        private SecondaryViewModel _dataContext;

        public SecondaryWindow()
        {
            InitializeComponent();
        }

        private void ModalWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_dataContext != null)
            {
                _dataContext.OnRequestClose -= _dataContext_OnRequestClose;
            }
            if (this.DataContext as SecondaryViewModel != null)
            {
                this._dataContext = this.DataContext as SecondaryViewModel;
                this._dataContext.OnRequestClose += _dataContext_OnRequestClose;
            }
        }

        private void _dataContext_OnRequestClose(object sender, Events.CloseViewModelEvent args)
        {
            this.Close();
        }
    }
}
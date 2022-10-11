using System.Windows;
using UnifiedDataExplorer.ViewModel;

namespace UnifiedDataExplorer
{
    public partial class ModalWindow : Window
    {
        private ModalViewModel _dataContext;

        public ModalWindow()
        {
            InitializeComponent();
        }

        private void ModalWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_dataContext != null)
            {
                _dataContext.OnRequestClose -= _dataContext_OnRequestClose;
            }
            if (this.DataContext as ModalViewModel != null)
            {
                this._dataContext = this.DataContext as ModalViewModel;
                this._dataContext.OnRequestClose += _dataContext_OnRequestClose;
            }
        }

        private void _dataContext_OnRequestClose(object sender, Events.CloseViewModelEvent args)
        {
            this.Close();
        }
    }
}

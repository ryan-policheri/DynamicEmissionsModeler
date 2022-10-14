using System.ComponentModel;
using System.Windows;
using UnifiedDataExplorer.ViewModel;

namespace UnifiedDataExplorer
{
    public partial class ModalWindow : Window
    {
        private ModalViewModel ViewModel;
        private bool ClosedByViewModel = false;

        public ModalWindow()
        {
            InitializeComponent();
        }

        private void ModalWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.OnRequestClose -= ViewModelOnRequestClose;
            }
            if (this.DataContext as ModalViewModel != null)
            {
                this.ViewModel = this.DataContext as ModalViewModel;
                this.ViewModel.OnRequestClose += ViewModelOnRequestClose;
            }
        }

        private void ViewModelOnRequestClose(object sender, Events.CloseViewModelEvent args)
        {
            ClosedByViewModel = true;
            this.Close();
        }

        private void ModalWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //if(!ClosedByViewModel) e.Cancel = true;
            //this.ViewModel.CancelProxyCommand.Execute(null);
        }
    }
}

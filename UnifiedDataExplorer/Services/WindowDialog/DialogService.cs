using System;
using System.Windows;
using UnifiedDataExplorer.ViewModel;

namespace UnifiedDataExplorer.Services.Window
{
    public class DialogService : IDialogService
    {
        private readonly Func<ModalViewModel> _modalViewModelFactory;
        private readonly Func<SecondaryViewModel> _secondaryVmFactory;
        private MainWindow _mainWindow;

        public DialogService(Func<ModalViewModel> modalViewModelFactory, Func<SecondaryViewModel> secondaryVmFactory)
        {
            _modalViewModelFactory = modalViewModelFactory;
            _secondaryVmFactory = secondaryVmFactory;
        }

        public T ShowModalWindow<T>(T dataContext)
        {
            var modalVm = _modalViewModelFactory();
            modalVm.ChildViewModel = dataContext;

            _mainWindow = (MainWindow)App.Current.MainWindow;
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.DataContext = modalVm;
            modalWindow.Owner = _mainWindow;
            modalWindow.Left = _mainWindow.Left + _mainWindow.Width / 4;
            modalWindow.Top = _mainWindow.Top + _mainWindow.Height / 4;
            modalWindow.Width = _mainWindow.ActualWidth / 2;
            modalWindow.Height = _mainWindow.ActualHeight / 2;
            modalWindow.Title = typeof(T).Name;
            modalWindow.ShowDialog();

            return dataContext;
        }

        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message); //TODO: Use own window
        }

        public void ShowInfoMessage(string message)
        {
            MessageBox.Show(message); //TODO: Use own window
        }

        public void OpenSecondaryWindow<T>(T dataContext)
        {
            var secondaryVm = _secondaryVmFactory();
            secondaryVm.ChildViewModel = dataContext;

            _mainWindow = (MainWindow)App.Current.MainWindow;
            SecondaryWindow secondaryWindow = new SecondaryWindow();
            secondaryWindow.DataContext = secondaryVm;
            secondaryWindow.Owner = _mainWindow;
            secondaryWindow.Left = _mainWindow.Left + _mainWindow.Width / 3;
            secondaryWindow.Top = _mainWindow.Top + _mainWindow.Height / 3;
            secondaryWindow.Width = _mainWindow.ActualWidth / 1.5;
            secondaryWindow.Height = _mainWindow.ActualHeight / 1.5;
            secondaryWindow.Title = typeof(T).Name;
            secondaryWindow.Show();
        }
    }
}

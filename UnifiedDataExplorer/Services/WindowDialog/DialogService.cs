using DotNetCommon.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UnifiedDataExplorer.Services.Window
{
    public class DialogService : IDialogService
    {
        private MainWindow _mainWindow;

        public DialogService()
        {
        }

        public T ShowModalWindow<T>(T dataContext)
        {
            _mainWindow = (MainWindow)App.Current.MainWindow;
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.DataContext = dataContext;
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
    }
}

using DotNetCommon.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedDataExplorer.Services.Window
{
    public class DialogService : IDialogService
    {
        private MainWindow _mainWindow;

        public DialogService()
        {
            _mainWindow = (MainWindow)App.Current.MainWindow;
        }

        public T ShowModalWindow<T>(T dataContext)
        {
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.DataContext = dataContext;
            modalWindow.Owner = _mainWindow;
            modalWindow.ShowDialog();

            return dataContext;
        }
    }
}

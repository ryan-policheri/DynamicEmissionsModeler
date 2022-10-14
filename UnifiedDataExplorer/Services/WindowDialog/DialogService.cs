using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using UnifiedDataExplorer.ViewModel;

namespace UnifiedDataExplorer.Services.WindowDialog
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
            secondaryWindow.Left = _mainWindow.Left + _mainWindow.Width / 4;
            secondaryWindow.Top = _mainWindow.Top + _mainWindow.Height / 4;
            secondaryWindow.Width = _mainWindow.ActualWidth / 1.5;
            secondaryWindow.Height = _mainWindow.ActualHeight / 1.5;
            secondaryWindow.Title = typeof(T).Name;
            secondaryWindow.Show();
        }

        //Modal shows on main windows and blocks all windows until modal completes
        public T ShowModalWindow<T>(T dataContext)
        {
            var modalVm = _modalViewModelFactory();
            modalVm.ChildViewModel = dataContext;

            System.Windows.Window currentWindow = App.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(x => x.IsActive);
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.DataContext = modalVm;
            modalWindow.Owner = currentWindow;
            modalWindow.Left = currentWindow.Left + currentWindow.Width / 4;
            modalWindow.Top = currentWindow.Top + currentWindow.Height / 4;
            modalWindow.Width = currentWindow.ActualWidth / 2;
            modalWindow.Height = currentWindow.ActualHeight / 2;
            modalWindow.Title = typeof(T).Name;
            modalWindow.ShowDialog();

            return dataContext;
        }


        //Modal shows on current window and does not block other windows (other than the current)
        //https://stackoverflow.com/questions/2593498/wpf-modal-window-using-showdialog-blocks-all-other-windows
        public void ShowModalWindow<T>(T dataContext, Action callback, ModalOptions options = null)
        {
            if (options == null) options = ModalOptions.DefaultModalOptions;

            //get current window
            System.Windows.Window currentWindow = App.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(x => x.IsActive);

            //prepare modal window
            var modalVm = _modalViewModelFactory();
            modalVm.ChildViewModel = dataContext;
            modalVm.ClosingCallback = callback;
            modalVm.Options = options;

            //prepare modal window
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.DataContext = modalVm;
            modalWindow.Owner = currentWindow;
            modalWindow.Left = currentWindow.Left + currentWindow.Width / 4;
            modalWindow.Top = currentWindow.Top + currentWindow.Height / 4;
            modalWindow.Width = currentWindow.ActualWidth / 2;
            modalWindow.Height = currentWindow.ActualHeight / 2;
            modalWindow.Title = typeof(T).Name;

            // get parent window handle
            IntPtr parentHandle = (new WindowInteropHelper(modalWindow.Owner)).Handle;
            // disable parent window
            EnableWindow(parentHandle, false);
            // when the dialog is closing we want to re-enable the parent
            modalWindow.Closing += ModalDialogWindow_Closing;
            // wait for the dialog window to be closed
            new ShowAndWaitHelper(modalWindow).ShowAndWait();
            modalWindow.Owner.Activate();
        }

        /// <summary>
        /// Enables or disables mouse and keyboard input to the specified window or control. 
        /// When input is disabled, the window does not receive input such as mouse clicks and key presses. 
        /// When input is enabled, the window receives all input.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        private void ModalDialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var win = (ModalWindow)sender;
            win.Closing -= ModalDialogWindow_Closing;
            IntPtr winHandle = (new WindowInteropHelper(win)).Handle;
            EnableWindow(winHandle, false);

            if (win.Owner != null)
            {
                IntPtr parentHandle = (new WindowInteropHelper(win.Owner)).Handle;
                // reenable parent window
                EnableWindow(parentHandle, true);
            }
            
        }

        private sealed class ShowAndWaitHelper
        {
            private readonly System.Windows.Window _window;
            private DispatcherFrame _dispatcherFrame;
            internal ShowAndWaitHelper(System.Windows.Window window)
            {
                if (window == null)
                {
                    throw new ArgumentNullException("window");
                }
                _window = window;
            }
            internal void ShowAndWait()
            {
                if (_dispatcherFrame != null)
                {
                    throw new InvalidOperationException("Cannot call ShowAndWait while waiting for a previous call to ShowAndWait to return.");
                }
                _window.Closed += OnWindowClosed;
                _window.Show();
                _dispatcherFrame = new DispatcherFrame();
                Dispatcher.PushFrame(_dispatcherFrame);
            }
            private void OnWindowClosed(object source, EventArgs eventArgs)
            {
                if (_dispatcherFrame == null)
                {
                    return;
                }
                _window.Closed -= OnWindowClosed;
                _dispatcherFrame.Continue = false;
                _dispatcherFrame = null;
            }
        }
    }
}

using System;

namespace UnifiedDataExplorer.Services.WindowDialog
{
    public interface IDialogService
    {
        T ShowModalWindow<T>(T dataContext, ModalOptions options = null);
        void ShowModalWindow<T>(T dataContext, Action callback, ModalOptions options = null);
        void ShowErrorMessage(string message);
        void ShowInfoMessage(string message);
        void OpenSecondaryWindow<T>(T dataContext);
    }
}
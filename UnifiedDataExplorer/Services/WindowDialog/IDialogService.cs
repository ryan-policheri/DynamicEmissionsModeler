namespace UnifiedDataExplorer.Services.Window
{
    public interface IDialogService
    {
        T ShowModalWindow<T>(T dataContext);
        void ShowErrorMessage(string message);
        void ShowInfoMessage(string message);
        void OpenSecondaryWindow<T>(T dataContext);
    }
}
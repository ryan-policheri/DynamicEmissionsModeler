using DotNetCommon.MVVM;

namespace UnifiedDataExplorer.Services.Window
{
    public interface IDialogService
    {
        T ShowModalWindow<T>(T dataContext);
    }
}
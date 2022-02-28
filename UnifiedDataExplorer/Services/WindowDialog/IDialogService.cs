using DotNetCommon.MVVM;

namespace UnifiedDataExplorer.Services
{
    public interface IDialogService
    {
        T ShowModalWindow<T>(T dataContext);
    }
}
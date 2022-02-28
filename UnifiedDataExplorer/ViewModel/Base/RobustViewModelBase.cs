using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using Microsoft.Extensions.Logging;
using UIowaBuildingsModel;
using UnifiedDataExplorer.Services;

namespace UnifiedDataExplorer.ViewModel.Base
{
    public class RobustViewModelBase : ViewModelBase
    {
        private readonly RobustViewModelDependencies _facade;

        public RobustViewModelBase(RobustViewModelDependencies facade)
        {
            _facade = facade;
        }

        protected IMessageHub MessageHub => _facade.MessageHub;

        protected ILogger Logger => _facade.Logger;

        protected IDialogService DialogService => _facade.DialogService;

        protected DataFileProvider DataFileProvider => _facade.DataFileProvider;

        protected T Resolve<T>()
        {
            return (T)_facade.ServiceProvider.GetService(typeof(T));
        }
    }
}

using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using Microsoft.Extensions.Logging;

namespace EIADataViewer.ViewModel.Base
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

        protected T Resolve<T>()
        {
            return (T)_facade.ServiceProvider.GetService(typeof(T));
        }
    }
}

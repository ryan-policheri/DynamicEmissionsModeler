using System;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using Microsoft.Extensions.Logging;
using UnifiedDataExplorer.Services.DataPersistence;
using UnifiedDataExplorer.Services.Window;

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
            T result = (T)_facade.ServiceProvider.GetService(typeof(T));
            if (result == null) throw new InvalidOperationException($"Unable to find type {typeof(T).Name}. Ensure is it registered with the service provider");
            else return result;
        }
    }
}

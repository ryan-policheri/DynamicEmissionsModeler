using System;
using DotNetCommon.EventAggregation;
using Microsoft.Extensions.Logging;

namespace EIADataViewer.ViewModel.Base
{
    public class RobustViewModelDependencies
    {
        public RobustViewModelDependencies(IServiceProvider serviceProvider, IMessageHub messageHub, ILogger logger)
        {
            ServiceProvider = serviceProvider;
            MessageHub = messageHub;
            Logger = logger;
        }

        public IServiceProvider ServiceProvider { get; }

        public IMessageHub MessageHub { get; }

        public ILogger Logger { get; }
    }
}

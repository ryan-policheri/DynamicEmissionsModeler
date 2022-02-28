using System;
using Microsoft.Extensions.Logging;
using DotNetCommon.EventAggregation;
using UIowaBuildingsModel;

namespace UnifiedDataExplorer.ViewModel.Base
{
    public class RobustViewModelDependencies
    {
        public RobustViewModelDependencies(IServiceProvider serviceProvider, IMessageHub messageHub, ILogger logger, DataFileProvider provider)
        {
            ServiceProvider = serviceProvider;
            MessageHub = messageHub;
            Logger = logger;
            DataFileProvider = provider;
        }

        public IServiceProvider ServiceProvider { get; }

        public IMessageHub MessageHub { get; }

        public ILogger Logger { get; }

        public DataFileProvider DataFileProvider { get; }
    }
}
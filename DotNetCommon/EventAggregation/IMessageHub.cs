using System;

namespace DotNetCommon.EventAggregation
{
    public interface IMessageHub : IDisposable
    {//Source: https://github.com/NimaAra/Easy.MessageHub
        void RegisterGlobalHandler(Action<Type, object> onMessage);
        void RegisterGlobalErrorHandler(Action<Guid, Exception> onError);
        void Publish<T>(T message);
        Guid Subscribe<T>(Action<T> action);
        Guid Subscribe<T>(Action<T> action, TimeSpan throttleBy);
        void Unsubscribe(Guid token);
        bool IsSubscribed(Guid token);
        void ClearSubscriptions();
    }
}

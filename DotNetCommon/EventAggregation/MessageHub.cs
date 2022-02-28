using System;
using System.Diagnostics;
using System.Reflection;

namespace DotNetCommon.EventAggregation
{
    public sealed class MessageHub : IMessageHub
    {//Source: https://github.com/NimaAra/Easy.MessageHub
        private readonly Subscriptions _subscriptions;
        private Action<Type, object> _globalHandler;
        private Action<Guid, Exception> _globalErrorHandler;

        public MessageHub() => _subscriptions = new Subscriptions();

        public void RegisterGlobalHandler(Action<Type, object> onMessage)
        {
            EnsureNotNull(onMessage);
            _globalHandler = onMessage;
        }

        public void RegisterGlobalErrorHandler(Action<Guid, Exception> onError)
        {
            EnsureNotNull(onError);
            _globalErrorHandler = onError;
        }

        public void Publish<T>(T message)
        {
            var localSubscriptions = _subscriptions.GetTheLatestSubscriptions();

            var msgType = typeof(T);

            var msgTypeInfo = msgType.GetTypeInfo();
            _globalHandler?.Invoke(msgType, message);

            // ReSharper disable once ForCanBeConvertedToForeach | Performance Critical
            for (var idx = 0; idx < localSubscriptions.Count; idx++)
            {
                var subscription = localSubscriptions[idx];

                if (!subscription.Type.GetTypeInfo().IsAssignableFrom(msgTypeInfo)) { continue; }
                try
                {
                    subscription.Handle(message);
                }
                catch (Exception e)
                {
                    _globalErrorHandler?.Invoke(subscription.Token, e);
                }
            }
        }

        public Guid Subscribe<T>(Action<T> action) => Subscribe(action, TimeSpan.Zero);

        public Guid Subscribe<T>(Action<T> action, TimeSpan throttleBy)
        {
            EnsureNotNull(action);
            return _subscriptions.Register(throttleBy, action);
        }

        public void Unsubscribe(Guid token) => _subscriptions.UnRegister(token);

        public bool IsSubscribed(Guid token) => _subscriptions.IsRegistered(token);

        public void ClearSubscriptions() => _subscriptions.Clear(false);

        public void Dispose()
        {
            _globalHandler = null;
            _globalErrorHandler = null;
            _subscriptions.Clear(true);
        }

        [DebuggerStepThrough]
        private void EnsureNotNull(object obj)
        {
            if (obj is null) { throw new NullReferenceException(nameof(obj)); }
        }
    }
}

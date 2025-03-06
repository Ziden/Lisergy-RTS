using System;
using System.Collections.Generic;

namespace Game.Engine.Events.Bus
{
    public class EventBus<T>
    {
        public event Action<T> OnEventFired;

        private readonly Dictionary<Type, List<ListenerWrapper>> _listeners = new Dictionary<Type, List<ListenerWrapper>>();

        public void Call(T ev)
        {
            OnEventFired?.Invoke(ev);
            var eventType = ev.GetType();
            if (_listeners.TryGetValue(eventType, out var listeners))
            {
                foreach (var listenerWrapper in listeners)
                {
                    ((Action<object>)listenerWrapper.Callback)(ev);
                }
            }
        }

        public void On<EvType>(IEventListener listener, Action<EvType> callback)
        {
            var eventType = typeof(EvType);
            if (!_listeners.ContainsKey(eventType))
            {
                _listeners[eventType] = new List<ListenerWrapper>();
            }
            _listeners[eventType].Add(new ListenerWrapper(listener, (Action<object>)(ev => callback((EvType)ev))));
        }

        public void RemoveListener(IEventListener listener)
        {
            foreach (var key in _listeners.Keys)
            {
                _listeners[key].RemoveAll(l => l.Listener == listener);
            }
        }
    }
}

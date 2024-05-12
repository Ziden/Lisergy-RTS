using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.Engine.Events.Bus
{

    public interface IEventBusRegistry<EventType>
    {
        void Register<EvType>(IEventListener listener, Action<EvType> callback);
    }

    public class EventBus<EventType> : IEventBusRegistry<EventType>
    {
        public event Action<EventType> OnEventFired;

        internal List<RegisteredListener> _allListeners = new List<RegisteredListener>();
        internal Dictionary<Type, List<RegisteredListener>> _registeredListeners = new Dictionary<Type, List<RegisteredListener>>();


        public virtual void Call(EventType ev)
        {
            OnEventFired?.Invoke(ev);

            if (!_registeredListeners.ContainsKey(ev.GetType()))
            {
                if (!_registeredListeners.ContainsKey(ev.GetType().BaseType))
                {
                    return;
                }
            }

            List<RegisteredListener> registeredEvents = _registeredListeners[ev.GetType()];
            foreach (RegisteredListener registeredEvent in registeredEvents)
            {
                registeredEvent.Call(ev);
            }

        }


        public virtual void Register<EvType>(IEventListener listener, Action<EvType> callback)
        {
            var eventType = typeof(EvType);
            if (!_registeredListeners.ContainsKey(eventType))
            {
                _registeredListeners.Add(eventType, new List<RegisteredListener>());
            }
            List<RegisteredListener> eventList = _registeredListeners[eventType];
            var registry = new RegisteredListener()
            {
                Method = callback,
                Listener = listener,
                Type = eventType
            };
            eventList.Add(registry);
            _allListeners.Add(registry);
        }


        public virtual void RemoveListener(IEventListener listener)
        {
            foreach (var l in new List<RegisteredListener>(_allListeners))
            {
                if (l.Listener == listener)
                {
                    _registeredListeners[l.Type].Remove(l);
                    _allListeners.Remove(l);
                }
            }
        }
    }
}

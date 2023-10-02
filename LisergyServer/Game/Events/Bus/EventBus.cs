using Game.Player;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.Events.Bus
{

    public class EventBus<EventType>
    {
        public event Action<BaseEvent> OnEventFired;

        internal Dictionary<Type, List<RegisteredListener>> _registeredListeners;

        public virtual void Call(BaseEvent ev)
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

        public EventBus()
        {
            Clear();
        }

        public void Clear()
        {
            _registeredListeners = new Dictionary<Type, List<RegisteredListener>>();
        }

        public void Clear(IEventListener listener)
        {
            // TODO
        }

        public virtual void Register<EvType>(IEventListener listener, Action<EvType> callback)
        {
            var eventType = typeof(EvType);
            if (!_registeredListeners.ContainsKey(eventType))
            {
                _registeredListeners.Add(eventType, new List<RegisteredListener>());
            }
            List<RegisteredListener> eventList = _registeredListeners[eventType];
            eventList.Add(new RegisteredListener()
            {
                Method = callback,
                Listener = new WeakReference<IEventListener>(listener),
                Type = eventType
            }); ;
        }
    }
}

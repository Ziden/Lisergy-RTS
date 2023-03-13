using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Events.Bus
{
    public class EventBus
    {
        private Dictionary<Type, List<RegisteredListener>> _registeredListeners;
        private HashSet<IEventListener> _listeners;

        public void RunCallbacks(PlayerEntity sender, byte [] eventBytes)
        {
            var ev = Serialization.ToEventRaw(eventBytes);
            ev.Sender = sender;
            Call(ev);
        }

        public void Call(BaseEvent ev)
        {
            if (!_registeredListeners.ContainsKey(ev.GetType()))
                if (!_registeredListeners.ContainsKey(ev.GetType().BaseType))
                    return;

            var registeredEvents = _registeredListeners[ev.GetType()];
            foreach (var registeredEvent in registeredEvents)
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
            _listeners = new HashSet<IEventListener>();
        }



        private void RegisterCallback(IEventListener listener, MethodInfo method, Type eventType)
        {
            if (!_registeredListeners.ContainsKey(eventType))
            {
                _registeredListeners.Add(eventType, new List<RegisteredListener>());
            }
            var eventList = _registeredListeners[eventType];
            eventList.Add(new RegisteredListener()
            {
                Method = method,
                Listener = listener
            });
            _listeners.Add(listener);
        }

        public void Register<EventType>(IEventListener listener, Action<EventType> callback)
        {
            if(!callback.Method.CustomAttributes.Any(a => a.AttributeType == typeof(EventMethod)))
            {
                throw new Exception("Listener must have [EventMethod] Attribute");
            }
            RegisterCallback(listener, callback.Method, typeof(EventType));
        }

        /*
        /// Registers all callbacks from listener.
        /// Removed as it was not really clear.
        public void RegisterListener(IEventListener listener)
        {
            var type = listener.GetType();
            var methods = type.GetMethods();
            foreach (var method in type.GetMethods())
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var parameter = parameters[0];
                    var hasAttribute = method.GetCustomAttributes(typeof(EventMethod), false).Any();

                    if (hasAttribute && typeof(BaseEvent).IsAssignableFrom(parameter.ParameterType))
                    {
                        RegisterCallback(listener, method, parameter.ParameterType);
                    }
                }
            }
        }
        */
    }
}

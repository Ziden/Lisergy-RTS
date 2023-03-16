using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Tests")]
namespace Game.Events.Bus
{
    public class EventBus<EventType> 
    {

        internal Dictionary<Type, List<RegisteredListener>> _registeredListeners;
        internal HashSet<IEventListener> _listeners;

        public void RunCallbacks(PlayerEntity sender, byte [] eventBytes)
        {
            var ev = Serialization.ToEventRaw(eventBytes);
            ev.Sender = sender;
            Call(ev);
        }

        public virtual void Call(BaseEvent ev)
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

        public void Clear(IEventListener listener)
        {
           // TODO
        }

        private void RegisterCallback(IEventListener listener, Delegate del, Type eventType)
        {
            if (!_registeredListeners.ContainsKey(eventType))
            {
                _registeredListeners.Add(eventType, new List<RegisteredListener>());
            }
            var eventList = _registeredListeners[eventType];
            eventList.Add(new RegisteredListener()
            {
                Method = del,
                Listener = new WeakReference<IEventListener>(listener),
                Type = eventType
            }); ;
            _listeners.Add(listener);
        }

        public virtual void Register<EvType>(IEventListener listener, Action<EvType> callback)
        {

            RegisterCallback(listener, callback, typeof(EvType));
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

using System;

namespace Game.Engine.Events.Bus
{
    public class ListenerWrapper
    {
        public IEventListener Listener { get; }
        public Delegate Callback { get; }

        public ListenerWrapper(IEventListener listener, Delegate callback)
        {
            Listener = listener;
            Callback = callback;
        }
    }
}

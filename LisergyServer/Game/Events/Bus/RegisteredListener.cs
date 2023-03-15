using Game.Events.GameEvents;
using System;
using System.Reflection;

namespace Game.Events.Bus
{
    public class RegisteredListener
    {
        public WeakReference<IEventListener> Listener;
        public Delegate Method;
        public Type Type;

        public void Call<T>(T ev) where T : BaseEvent
        {
            Method.DynamicInvoke(ev);
        }
    }
}

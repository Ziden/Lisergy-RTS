using System;

namespace Game.Events.Bus
{
    public class RegisteredListener
    {
        public WeakReference<IEventListener> Listener;
        public Delegate Method;
        public Type Type;

        public void Call<T>(T ev) where T : BaseEvent
        {
            _ = Method.DynamicInvoke(ev);
        }
    }
}

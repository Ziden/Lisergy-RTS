using System;

namespace Game.Events.Bus
{
    public class RegisteredListener
    {
        public WeakReference<IEventListener> Listener;
        public Delegate Method;
        public Type Type;

        public void Call<T>(T ev) 
        {
            _ = Method.DynamicInvoke(ev);
        }
    }
}

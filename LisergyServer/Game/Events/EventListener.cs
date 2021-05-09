using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Listeners
{
    public abstract class EventListener : IDisposable
    {
        public EventListener()
        {
            Register();
        }

        public void Dispose()
        {
            Unregister();
        }

        public abstract void Register();

        public abstract void Unregister();
    }
}

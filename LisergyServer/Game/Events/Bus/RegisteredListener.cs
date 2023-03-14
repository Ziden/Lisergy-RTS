using Game.Events.GameEvents;
using System.Reflection;

namespace Game.Events.Bus
{
    public class RegisteredListener
    {
        public IEventListener Listener;
        public MethodInfo Method;

        public void Call(BaseEvent ev)
        {
            if(ev.GetType() == typeof(OffensiveMoveEvent))
            {
                var asd = 123;
            }
            Method.Invoke(Listener, new object[] { ev });
        }
    }
}

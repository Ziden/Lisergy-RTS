using System.Reflection;

namespace Game.Events.Bus
{
    public class RegisteredListener
    {
        public IEventListener Listener;
        public MethodInfo Method;

        public void Call(GameEvent ev)
        {
            Method.Invoke(Listener, new object[] { ev });
        }
    }
}

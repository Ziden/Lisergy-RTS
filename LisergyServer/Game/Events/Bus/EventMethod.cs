using System;

namespace Game.Events.Bus
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class EventMethod : Attribute
    {
    }
}

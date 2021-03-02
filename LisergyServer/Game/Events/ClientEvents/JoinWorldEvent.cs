using System;
using ZeroFormatter;

namespace Game.Events
{
    [Serializable]
    public class JoinWorldEvent : ClientEvent
    {
        public override EventID GetID() => EventID.JOIN;
    }
}

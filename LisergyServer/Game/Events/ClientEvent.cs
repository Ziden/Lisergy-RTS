using System;
using ZeroFormatter;

namespace Game.Events
{
    [Serializable]
    public abstract class ClientEvent: GameEvent
    {
        [NonSerialized]
        public int ConnectionID;

        [NonSerialized]
        public PlayerEntity Player;
    }
}

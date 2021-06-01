
using System;

namespace Game.Events
{
    [Serializable]
    public abstract class GameEvent
    {
        [NonSerialized]
        public int ConnectionID;

        [NonSerialized]
        public PlayerEntity Sender;
    }
}

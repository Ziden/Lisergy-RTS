
using System;

namespace Game.Events
{
    [Serializable]
    public abstract class GameEvent
    {
        public abstract EventID GetID();

        [NonSerialized]
        public int ConnectionID;

        [NonSerialized]
        public PlayerEntity ClientPlayer;
    }
}

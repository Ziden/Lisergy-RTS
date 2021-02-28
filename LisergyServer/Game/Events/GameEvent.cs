
using System;

namespace Game.Events
{
    [Serializable]
    public abstract class GameEvent
    {
        public abstract EventID GetID();
        public bool FromNetwork; // TODO- REMOVE

        [NonSerialized]
        public int ConnectionID;

        [NonSerialized]
        public PlayerEntity Sender;
    }
}


using Game.Entity.Entities;
using System;

namespace Game.Events
{
    [Serializable]
    public abstract class BaseEvent
    {
        [NonSerialized]
        public int ConnectionID;

        [NonSerialized]
        public PlayerEntity Sender;
    }
}

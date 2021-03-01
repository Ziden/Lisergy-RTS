using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityVisibleEvent : ServerEvent
    {
        public EntityVisibleEvent(WorldEntity entity)
        {
            this.Entity = entity;
        }

        public WorldEntity Entity;
     
        public override EventID GetID() => EventID.PARTY_VISIBLE;

        public override string ToString()
        {
            return $"<EntityVisible {Entity}>";
        }
    }
}

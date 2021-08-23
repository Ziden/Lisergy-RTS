using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityVisiblePacket : ServerEvent
    {
        public EntityVisiblePacket(WorldEntity entity)
        {
            this.Entity = entity;
        }

        public WorldEntity Entity;
     
        public override string ToString()
        {
            return $"<EntityVisible {Entity}>";
        }
    }
}

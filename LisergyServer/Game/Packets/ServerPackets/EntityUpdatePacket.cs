using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityUpdatePacket : ServerPacket
    {
        public EntityUpdatePacket(WorldEntity entity)
        {
            this.Entity = entity;
        }

        public WorldEntity Entity;
     
        public override string ToString()
        {
            return $"<EntityUpdate {Entity}>";
        }
    }
}

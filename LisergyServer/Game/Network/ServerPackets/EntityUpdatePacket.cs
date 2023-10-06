using Game.ECS;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityUpdatePacket : ServerPacket
    {
        public BaseEntity Entity;
        public IComponent [] SyncedComponents;

        public EntityUpdatePacket(BaseEntity entity)
        {
            Entity = entity;
        }

        public override string ToString()
        {
            return $"<EntityUpdate {Entity}>";
        }
    }
}

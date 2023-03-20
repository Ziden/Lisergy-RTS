using Game.ECS;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityUpdatePacket : ServerPacket
    {
        public WorldEntity Entity;
        public List<IComponent> SyncedComponents;

        public EntityUpdatePacket(WorldEntity entity)
        {
            Entity = entity;
        }

        public override string ToString()
        {
            return $"<EntityUpdate {Entity}>";
        }
    }
}

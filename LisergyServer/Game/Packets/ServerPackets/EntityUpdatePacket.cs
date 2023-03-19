using Game.ECS;
using Game.Entity;
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
            SyncedComponents = entity.Components.GetSyncedComponents();
        }
     
        public override string ToString()
        {
            return $"<EntityUpdate {Entity}>";
        }
    }
}

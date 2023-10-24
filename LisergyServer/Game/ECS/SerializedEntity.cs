using Game.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ECS
{
    [Serializable]
    public class SerializedEntity
    {
        public EntityType EntityType;
        public GameId EntityId;
        public GameId OwnerId;
        public IComponent[] Components;

        public SerializedEntity(IEntity entity)
        {
            EntityType = entity.EntityType;
            EntityId = entity.EntityId;
            OwnerId = entity.OwnerID;
            Components = ((ComponentSet)entity.Components).Pointers.ToArray();
        }
    }
}

using Game.Engine.DataTypes;
using Game.Entities;
using System;
using System.Linq;

namespace Game.Engine.ECLS
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
            Components = ((ComponentSet)entity.Components).AllComponents().ToArray();
        }
    }
}

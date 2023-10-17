using Game.DataTypes;
using Game.ECS;
using Game.Network;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityUpdatePacket : BasePacket, IServerPacket, IPooledPacket
    {
        public EntityType Type;
        public GameId EntityId;
        public GameId OwnerId;
        public IComponent [] SyncedComponents;

        public EntityUpdatePacket() { }

        public EntityUpdatePacket(BaseEntity entity)
        {
            Type = entity.EntityType;
            EntityId = entity.EntityId;
            OwnerId = entity.OwnerID;
        }

        public override string ToString()
        {
            return $"<EntityUpdate {EntityId}>";
        }
    }
}

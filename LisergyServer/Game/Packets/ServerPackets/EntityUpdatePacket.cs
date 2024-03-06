using Game.ECS;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using System;
using System.Linq;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityUpdatePacket : BasePacket, IServerPacket, IPooledPacket
    {
        public EntityType Type;
        public GameId EntityId;
        public GameId OwnerId;
        public IComponent [] SyncedComponents;
        public uint[] RemovedComponentIds;

        public EntityUpdatePacket() { }

        public EntityUpdatePacket(BaseEntity entity)
        {
            Type = entity.EntityType;
            EntityId = entity.EntityId;
            OwnerId = entity.OwnerID;
        }

        public override string ToString()
        {
            return $"<EntityUpdate {EntityId} Components={string.Join(',', SyncedComponents.ToList())}>";
        }
    }
}

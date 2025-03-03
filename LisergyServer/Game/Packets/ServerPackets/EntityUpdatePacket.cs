using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Network;
using Game.Entities;
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
        public IComponent[] SyncedComponents;
        public uint[] RemovedComponentIds;

        public EntityUpdatePacket() { }

        public EntityUpdatePacket(IEntity entity)
        {
            Type = entity.EntityType;
            EntityId = entity.EntityId;
            OwnerId = entity.OwnerID;
        }

        public T GetComponent<T>()
        {
            return (T)SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(T));
        }

        public override string ToString()
        {
            return $"<EntityUpdate {Type} {EntityId} Components={string.Join(',', SyncedComponents.ToList())}>";
        }
    }
}

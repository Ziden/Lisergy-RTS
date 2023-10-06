using Game.DataTypes;
using Game.ECS;
using Game.Events;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class EntityDestroyPacket : ServerPacket
    {
        public EntityDestroyPacket(IEntity entity)
        {
            OwnerID = entity.OwnerID;
            EntityID = entity.EntityId;
        }

        public GameId OwnerID;
        public GameId EntityID;
    }
}

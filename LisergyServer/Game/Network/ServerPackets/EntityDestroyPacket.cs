using Game.DataTypes;
using Game.Events;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class EntityDestroyPacket : ServerPacket
    {
        public EntityDestroyPacket(BaseEntity entity)
        {
            OwnerID = entity.OwnerID;
            EntityID = entity.Id;
        }

        public GameId OwnerID;
        public GameId EntityID;
    }
}

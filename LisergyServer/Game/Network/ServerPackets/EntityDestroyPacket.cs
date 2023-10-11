using Game.DataTypes;
using Game.ECS;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class EntityDestroyPacket : BasePacket, IServerPacket
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

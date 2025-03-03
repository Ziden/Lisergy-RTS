using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Network;
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

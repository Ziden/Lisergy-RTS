using Game.DataTypes;
using System;

namespace Game.Events
{
    [Serializable]
    public class EntityDestroyPacket : ServerPacket
    {
        public EntityDestroyPacket(WorldEntity entity)
        {
            this.OwnerID = entity.OwnerID;
            this.EntityID = entity.Id;
        }

        public GameId OwnerID;
        public GameId EntityID;
    }
}

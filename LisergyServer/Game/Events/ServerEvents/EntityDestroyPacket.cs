using Game.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events
{
    [Serializable]
    public class EntityDestroyPacket : ServerEvent
    {
        public EntityDestroyPacket(WorldEntity entity)
        {
            this.OwnerID = entity.OwnerID;
            this.EntityID = entity.Id;
        }

        public string OwnerID;
        public string EntityID;
    }
}

using Game.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events
{
    [Serializable]
    public class EntityDestroyEvent : ServerEvent
    {
        public EntityDestroyEvent(WorldEntity entity)
        {
            this.OwnerID = entity.OwnerID;
            this.ID = entity.Id;
        }

        public string OwnerID;
        public string ID;
    }
}

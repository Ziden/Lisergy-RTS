using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class EntityVisibleEvent : ServerEvent
    {
        public EntityVisibleEvent(WorldEntity party, WorldEntity viewer)
        {
            this.Party = party;
            this.Viewer = viewer;
        }

        public WorldEntity Party;

        [NonSerialized]
        public WorldEntity Viewer;
     
        public override EventID GetID() => EventID.PARTY_VISIBLE;
    }
}

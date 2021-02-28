using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PartyVisibleEvent : ServerEvent
    {
        public PartyVisibleEvent(Party party, WorldEntity viewer)
        {
            this.Party = party;
            this.Viewer = viewer;
        }

        public Party Party;

        [NonSerialized]
        public WorldEntity Viewer;
     
        public override EventID GetID() => EventID.PARTY_VISIBLE;
    }
}

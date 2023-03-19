using Game.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class PartyStatusUpdateEvent : GameEvent
    {
        public PartyEntity Party;

        public PartyStatusUpdateEvent(PartyEntity p)
        {
            this.Party = p;
        }

    }
}

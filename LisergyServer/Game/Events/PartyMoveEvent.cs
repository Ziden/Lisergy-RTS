using Game.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events
{
    [Serializable]
    public class PartyMoveEvent : ServerEvent
    {
        public override EventID GetID() => EventID.PARTY_MOVE;

        public PartyMoveEvent(Party party, Tile tile)
        {
            this.OwnerID = party.OwnerID;
            this.PartyIndex = party.PartyIndex;
            this.Delay = party.MoveDelay;
            this.X = tile.X;
            this.Y = tile.Y;
        }

        public string OwnerID;
        public int PartyIndex;
        public TimeSpan Delay;
        public int X;
        public int Y;
    }
}

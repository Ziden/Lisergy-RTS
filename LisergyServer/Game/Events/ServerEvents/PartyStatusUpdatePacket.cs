using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PartyStatusUpdatePacket : ServerEvent
    {
        public GameId OwnerID;
        public byte PartyIndex;
        UnitStats[] Stats;

        public PartyStatusUpdatePacket(Party entity)
        {
            PartyIndex = entity.PartyIndex;
            OwnerID = entity.OwnerID;
            var units = entity.GetUnits();
            Stats = new UnitStats[units.Length];
            for (var x = 0; x < units.Length; x++)
            {
                Stats[x] = units[x] == null ? null : units[x].Stats;
            }

        }

        public override string ToString()
        {
            return $"<PartyStats idx={PartyIndex}>";
        }
    }
}

using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PartyStatsUpdateEvent : ServerEvent
    {
        public PartyStatsUpdateEvent(Party entity)
        {
            this.PartyIndex = entity.PartyIndex;
            var units = entity.GetUnits();
            Stats = new UnitStats[units.Length];
            for (var x = 0; x < units.Length; x++)
            {
                Stats[x] = units[x] == null ? null : units[x].Stats;
            }

        }

        public byte PartyIndex;
        UnitStats[] Stats;

        public override string ToString()
        {
            return $"<PartyStats idx={PartyIndex}>";
        }
    }
}

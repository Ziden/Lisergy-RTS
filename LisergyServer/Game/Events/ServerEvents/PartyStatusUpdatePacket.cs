﻿using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PartyStatusUpdatePacket : ServerEvent
    {
        public byte PartyIndex;
        UnitStats[] Stats;

        public PartyStatusUpdatePacket(Party entity)
        {
            this.PartyIndex = entity.PartyIndex;
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
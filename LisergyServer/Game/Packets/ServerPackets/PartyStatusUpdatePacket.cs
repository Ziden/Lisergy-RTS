using Game.DataTypes;
using Game.Entity;
using System;
using System.Linq;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PartyStatusUpdatePacket : ServerPacket
    {
        public GameId OwnerID;
        public byte PartyIndex;
        UnitStats[] Stats;

        public PartyStatusUpdatePacket(Party entity)
        {
            PartyIndex = entity.PartyIndex;
            OwnerID = entity.OwnerID;
            var units = entity.GetUnits().Where(u => u != null).ToArray();;
            Stats = new UnitStats[units.Length];
            for (var x = 0; x < units.Length; x++)
            {
                Stats[x] = units[x].Stats;
            }

        }

        public override string ToString()
        {
            return $"<PartyStats idx={PartyIndex}>";
        }
    }
}

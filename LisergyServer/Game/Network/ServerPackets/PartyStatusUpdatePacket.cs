using Game.DataTypes;
using Game.Entity;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.Party;
using GameData.Specs;
using System;
using System.Linq;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class PartyStatusUpdatePacket : ServerPacket
    {
        public GameId OwnerID;
        public byte PartyIndex;
        private readonly UnitStats[] Stats;

        public PartyStatusUpdatePacket(PartyEntity entity)
        {
            PartyIndex = entity.PartyIndex;
            OwnerID = entity.OwnerID;
            Unit[] units = entity.BattleGroupLogic.GetUnits().Where(u => u != null).ToArray(); ;
            Stats = new UnitStats[units.Length];
            for (int x = 0; x < units.Length; x++)
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

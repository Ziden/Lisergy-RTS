using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Systems.Battle.Data;
using Game.World;
using System;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Packet sent to player whenever a battle started
    /// </summary>
    [Serializable]
    public class BattleStartPacket : BasePacket, IServerPacket
    {
        public Location Position;
        public GameId BattleID;
        public BattleTeamData Attacker;
        public BattleTeamData Defender;

        public BattleStartPacket(in GameId battleId, in Location position, in BattleTeamData attacker, in BattleTeamData defender)
        {
            BattleID = battleId;
            Attacker = attacker;
            Defender = defender;
            Position = position;
        }
    }
}

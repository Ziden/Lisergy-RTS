using Game.Battle.Data;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Map;
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
        public TileVector Position;
        public GameId BattleID;
        public BattleTeamData Attacker;
        public BattleTeamData Defender;

        public BattleStartPacket(in GameId battleId, in TileVector position, in BattleTeamData attacker, in BattleTeamData defender)
        {
            BattleID = battleId;
            Attacker = attacker;
            Defender = defender;
            Position = position;
        }
    }
}

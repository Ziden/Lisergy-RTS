using Game.DataTypes;
using Game.Network.ServerPackets;
using System;

namespace Game.Battle
{
    /// <summary>
    /// Sent to client to know about a battle.
    /// </summary>
    [Serializable]
    public class BattleHeader
    {
        public bool AttackerWins;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public DateTime Date;

        public BattleHeader() { }
    }
}

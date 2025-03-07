using Game.Engine.DataTypes;
using System;

namespace Game.Systems.Battle.Data
{
    /// <summary>
    /// Represents a minimal header of the battle.
    /// Contains the final state of the attacker and defender teams
    /// </summary>
    [Serializable]
    public class BattleHeader
    {
        public bool AttackerWins;
        public GameId BattleID;
        public BattleGroupData Attacker;
        public BattleGroupData Defender;
        public DateTime BattleTime;
    }
}

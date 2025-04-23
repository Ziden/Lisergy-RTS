using System;
using Game.Engine.DataTypes;

namespace Game.Systems.Battle.Data
{
    /// <summary>
    ///     Represents a minimal header of the battle.
    ///     Contains the final state of the attacker and defender teams
    /// </summary>
    [Serializable]
	public class BattleHeader
	{
		public BattleGroupData Attacker;
		public bool AttackerWins;
		public GameId BattleID;
		public DateTime BattleTime;
		public BattleGroupData Defender;
	}
}
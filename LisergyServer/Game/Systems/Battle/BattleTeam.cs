using Game.Engine.DataTypes;
using Game.Systems.Battle.Data;
using System;
using System.Linq;

namespace Game.Systems.Battle
{
    /// <summary>
    /// Represents a battle team which consists of an array of units.
    /// Since units are structs, it will allocate unit pointers to be used inside the battle.
    /// </summary>
    public unsafe class BattleTeam
    {
        /// <summary>
        /// Struct representing the battle input.
        /// Will be updated only in the end of the battle.
        /// </summary>
        public BattleGroupData TeamData;
        public GameId OwnerID => TeamData.OwnerID;
        public BattleUnit[] Units { get; private set; }
        public BattleTeam(in BattleGroupData data)
        {
            TeamData = data;
            Units = data.Units.Select(u => new BattleUnit(this, u)).ToArray();
        }

        /// <summary>
        /// Updates the team data with the result of the battle
        /// </summary>
        public void UpdateTeamData()
        {
            /*
            if (_memory == null) throw new Exception("Can only copy data once");
            _memory.FreeAndCopyResults(ref TeamData);
            _memory = null;
            return ref TeamData;
            */
        }

        public BattleUnit[] Alive => Units.Where(u => !u.Dead).ToArray();
        public bool AllDead => !Units.Any(u => !u.Dead);
        public override string ToString()
        {
            return $"<Team Owner={TeamData.OwnerID} Units={string.Join(",", Units.Select(u => u.ToString()).ToArray())}";
        }
    }
}

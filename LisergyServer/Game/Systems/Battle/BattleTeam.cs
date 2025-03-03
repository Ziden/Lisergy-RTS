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
        public BattleTeamData TeamData;
        private BattleTeamMemory _memory;
        public ref readonly GameId OwnerID => ref TeamData.OwnerID;
        public BattleUnit[] Units { get; private set; }
        public BattleTeam(in BattleTeamData data)
        {
            TeamData = data;
            _memory = new BattleTeamMemory(TeamData);
            Units = new BattleUnit[] {
                new BattleUnit(this, _memory.GetUnit(0), _memory.GetUnitState(0)),
                new BattleUnit(this, _memory.GetUnit(1), _memory.GetUnitState(1)),
                new BattleUnit(this, _memory.GetUnit(2), _memory.GetUnitState(2)),
                new BattleUnit(this, _memory.GetUnit(3), _memory.GetUnitState(3))
            };
        }

        /// <summary>
        /// Updates the team data with the result of the battle
        /// </summary>
        public ref BattleTeamData UpdateTeamData()
        {
            if (_memory == null) throw new Exception("Can only copy data once");
            _memory.FreeAndCopyResults(ref TeamData);
            _memory = null;
            return ref TeamData;
        }

        public BattleUnit[] Alive => Units.Where(u => !u.Dead).ToArray();
        public bool AllDead => !Units.Any(u => !u.Dead);
        public override string ToString()
        {
            return $"<Team Owner={TeamData.OwnerID} Units={string.Join(",", Units.Select(u => u.ToString()).ToArray())}";
        }
    }
}

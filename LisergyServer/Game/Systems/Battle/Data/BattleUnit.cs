using Game.Engine.DataTypes;
using Game.Systems.Battler;
using System;

namespace Game.Systems.Battle.Data
{
    /// <summary>
    /// Represents a unit inside a battle.
    /// Holds the pointer of a unit to perform manipulations on the unit data.
    /// Holds also a pointer to battle specific states of the unit.
    /// </summary>
    public unsafe class BattleUnit : IComparable<BattleUnit>
    {
        public Unit UnitData { get; private set; }
        public BattleTeam Team { get; private set; }
        public ushort RT { get; set; }
        public bool Dead => UnitData.Stats.HP <= 0;
        public GameId UnitID => UnitData.Id;
        public BattleUnit(BattleTeam team, in Unit unit)
        {
            Team = team;
            UnitData = unit;
            RT = MaxRT;
        }

        /// <summary>
        /// Sorting method based on RT.
        /// This is to be used in sorted sets to decide which battle unit acts first
        /// </summary>
        public int CompareTo(BattleUnit obj) => obj == this ? 0 : obj.RT >= RT ? -1 : 1;
        public ushort MaxRT => (ushort)Math.Max(1, 255 - UnitData.Stats.Speed);
        public override string ToString() => $"<BattleUnit RT={RT} {UnitData.ToString()}>";
    }
}

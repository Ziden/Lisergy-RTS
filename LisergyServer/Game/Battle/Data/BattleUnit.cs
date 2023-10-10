using Game.DataTypes;
using Game.Systems.Battler;
using Game.Systems.Player;
using System;

namespace Game.Battle.Data
{
    /// <summary>
    /// Represents a unit inside a battle.
    /// Holds the pointer of a unit to perform manipulations on the unit data.
    /// Holds also a pointer to battle specific states of the unit.
    /// </summary>
    public unsafe class BattleUnit : IComparable<BattleUnit>
    {
        public Unit* UnitPtr { get; private set; }
        public BattleUnitState* StatePtr { get; private set; }
        public BattleTeam Team { get; private set; }
        public ref ushort RT => ref StatePtr->RT;
        public bool Dead => UnitPtr->HP <= 0;
        public ref readonly GameId UnitID => ref UnitPtr->Id;
        public BattleUnit(BattleTeam team, in Unit* unit, in BattleUnitState* state)
        {
            Team = team;
            UnitPtr = unit;
            StatePtr = state;
            RT = MaxRT;
        }

        /// <summary>
        /// Sorting method based on RT.
        /// This is to be used in sorted sets to decide which battle unit acts first
        /// </summary>
        public int CompareTo(BattleUnit obj) => obj == this ? 0 : obj.RT >= RT ? -1 : 1;
        public ushort MaxRT => (ushort)Math.Max(1, 255 - UnitPtr->Speed);
        public override string ToString() => $"<BattleUnit RT={RT} {UnitPtr->ToString()}>";
    }
}

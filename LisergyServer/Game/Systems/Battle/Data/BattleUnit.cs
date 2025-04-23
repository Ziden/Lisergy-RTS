using System;
using Game.Engine.DataTypes;
using Game.Systems.Battler;
using Newtonsoft.Json;

namespace Game.Systems.Battle.Data
{
    /// <summary>
    ///     Represents a unit inside a battle.
    ///     Holds the pointer of a unit to perform manipulations on the unit data.
    ///     Holds also a pointer to battle specific states of the unit.
    /// </summary>
    public class BattleUnit : IComparable<BattleUnit>
	{
		public BattleUnit(BattleTeam team, in Unit unit)
		{
			Team = team;
			UnitData = unit;
			RT = MaxRT;
		}

		public Unit UnitData { get; }

		[JsonIgnore] public BattleTeam Team { get; private set; }

		public ushort RT { get; set; }
		public bool Dead => UnitData.Stats.HP <= 0;
		public GameId UnitID => UnitData.Id;
		public ushort MaxRT => (ushort) Math.Max(1, 255 - UnitData?.Stats.Speed ?? 0);

        /// <summary>
        ///     Sorting method based on RT.
        ///     This is to be used in sorted sets to decide which battle unit acts first
        /// </summary>
        public int CompareTo(BattleUnit obj)
		{
			return obj == this ? 0 : obj.RT >= RT ? -1 : 1;
		}

		public override string ToString()
		{
			return $"<BattleUnit RT={RT} {UnitData}>";
		}
	}
}
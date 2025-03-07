using Game.Engine.DataTypes;
using Game.Specs;
using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Systems.Battler
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Unit : IEquatable<Unit>, IEqualityComparer<Unit>
    {
        public GameId Id;
        public UnitSpecId SpecId;
        public UnitStats Stats;

        public Unit()
        {
        }

        public Unit(UnitSpec spec)
        {
            Id = GameId.Generate();
            SpecId = spec.SpecId;
            Stats = new UnitStats();
            Stats.SetStats(spec.Stats);
            HealAll();
        }

        /// <summary>
        /// Gets the unit HP ratio from 1 (100%) to 0 (no HP)
        /// </summary>
        public double HpRatio => Stats.HP / (double)Stats.MaxHP;

        public void HealAll()
        {
            Stats.HP = Stats.MaxHP;
            Stats.MP = Stats.MaxMP;
        }

        public override string ToString()
        {
            return !Valid ? "<Unit Null>" : $"<Unit Spec={SpecId}/>";
        }

        public bool Valid => Id != GameId.ZERO;

        public bool Equals(Unit other)
        {
            return other != null && other.Valid == Valid && SpecId == other.SpecId && Stats.Equals(other.Stats);
        }

        public bool Equals(Unit x, Unit y)
        {
            return x?.Valid == y?.Valid && x.Equals(y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecId, Stats);
        }

        public int GetHashCode(Unit obj)
        {
            return obj.GetHashCode();
        }
    }
}

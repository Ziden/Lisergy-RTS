using Game.Battle;
using Game.DataTypes;
using Game.Entity;
using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Systems.Battler
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Unit : IEquatable<Unit>, IEqualityComparer<Unit>
    {
        public GameId Id;
        public ushort SpecId;
        public UnitStats Stats;

        public Unit(UnitSpec spec)
        {
            Id = GameId.Generate();
            SpecId = spec.UnitSpecID;
            Stats = new UnitStats();
            Stats.SetStats(spec.Stats);
            HealAll();
        }

        public byte Atk { get => Stats.Atk; set => Stats.Atk = value; }
        public byte Def { get => Stats.Def; set => Stats.Def = value; }
        public byte Matk { get => Stats.Matk; set => Stats.Matk = value; }
        public byte Mdef { get => Stats.Mdef; set => Stats.Mdef = value; }
        public byte Speed { get => Stats.Speed; set => Stats.Speed = value; }
        public byte Accuracy { get => Stats.Accuracy; set => Stats.Accuracy = value; }
        public byte Weight { get => Stats.Weight; set => Stats.Weight = value; }
        public byte CargoWeight { get => Stats.CargoWeightBonusPct; set => Stats.CargoWeightBonusPct = value; }
        public byte HP { get => Stats.HP; set => Stats.HP = value; }
        public byte MaxHP { get => Stats.MaxHP; set => Stats.MaxHP = value; }
        public byte MP { get => Stats.MP; set => Stats.MP = value; }
        public byte MaxMP { get => Stats.MaxMP; set => Stats.MaxHP = value; }

        /// <summary>
        /// Gets the unit HP ratio from 1 (100%) to 0 (no HP)
        /// </summary>
        public float HpRatio => HP / (float)MaxHP;

        public void CopyFrom(Unit u)
        {
            var size = sizeof(Unit);
            var sourcePtr = &u;
            fixed (Unit* thisPtr = &this)
            {
                Buffer.MemoryCopy(sourcePtr, thisPtr, size, size);
            }
        }

        public void HealAll()
        {
            HP = MaxHP;
            MP = MaxMP;
        }

        public override string ToString()
        {
            return !Valid ? "<Unit Null>" : $"<Unit Spec={SpecId}/>";
        }

        public bool Valid => Id != GameId.ZERO;

        public bool Equals(Unit other)
        {
            return other.Valid == Valid && SpecId == other.SpecId && Stats.Equals(other.Stats);
        }

        public bool Equals(Unit x, Unit y)
        {
            return x.Valid == y.Valid && x.Equals(y);
        }

        public static bool operator ==(Unit c1, Unit c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Unit c1, Unit c2)
        {
            return !c1.Equals(c2);
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

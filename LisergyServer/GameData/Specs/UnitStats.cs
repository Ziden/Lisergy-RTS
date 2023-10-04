using GameData.buffs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Game.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public unsafe struct UnitStats
    {
        public readonly static UnitStats DEFAULT = new UnitStats()
        {
            Atk = 1,
            Def = 1,
            Matk = 1,
            Mdef = 1,
            Speed = 10,
            Accuracy = 10,
            Weight = 60,
            MaxHP = 5,
            HP = 5,
            MP = 2,
            MaxMP = 2,
            Move = 5
        };

        public byte Atk;
        public byte Def;
        public byte Matk;
        public byte Mdef;
        public byte Speed;
        public byte Accuracy;
        public byte Weight;
        public byte Move;
        public ushort HP;
        public ushort MaxHP;
        public ushort MP;
        public ushort MaxMP;

        public UnitStats SetStats(Dictionary<Stat, ushort> stats)
        {
            foreach (var kp in stats)
                this[kp.Key] = kp.Value;
            return this;
        }

        public void SetStats(UnitStats stats)
        {
            var size = sizeof(UnitStats);
            var sourcePtr = &stats;
            fixed (UnitStats* thisPtr = &this)
            {
                Buffer.MemoryCopy(sourcePtr, thisPtr, size, size);
            }
        }

        public ushort GetStat(Stat s)
        {
            return this[s];
        }

        public void SetStat(Stat stat, ushort value)
        {
            this[stat] = value;
        }

        public void AddStat(Stat stat, in ushort value)
        {
            this[stat] += value;
        }

        public void SubStat(Stat stat, in ushort value)
        {
            this[stat] -= value;
        }

        public ushort this[Stat stat]
        {
            get
            {
                switch (stat)
                {
                    case Stat.HP: return HP;
                    case Stat.MHP: return MaxHP;
                    case Stat.MP: return MP;
                    case Stat.MMP: return MaxMP;
                    case Stat.DEF: return Def;
                    case Stat.SPEED: return Speed;
                    case Stat.WEIGHT: return Weight;
                    case Stat.MATK: return Matk;
                    case Stat.MDEF: return Mdef;
                    case Stat.ACCURACY: return Accuracy;
                    case Stat.ATK: return Atk;
                    case Stat.MOVE: return Move;
                }
                throw new Exception("Invalid stat " + stat.ToString());
            }
            set
            {
                switch (stat)
                {
                    case Stat.HP: HP = value; break;
                    case Stat.MHP: MaxHP = value; break;
                    case Stat.MP: MP = value; break;
                    case Stat.MMP: MaxMP = value; break;
                    case Stat.DEF: Def = (byte)value; break;
                    case Stat.SPEED: Speed = (byte)value; break;
                    case Stat.WEIGHT: Weight = (byte)value; break;
                    case Stat.MATK: Matk = (byte)value; break;
                    case Stat.MDEF: Mdef = (byte)value; break;
                    case Stat.ACCURACY: Accuracy = (byte)value; break;
                    case Stat.ATK: Atk = (byte)value; break;
                    case Stat.MOVE: Move = (byte)value; break;
                    default: throw new Exception("Invalid stat " + stat.ToString());
                }

            }

        }
    }
}

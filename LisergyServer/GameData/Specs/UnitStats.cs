using GameData.buffs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public unsafe struct UnitStats
    {
        /// <summary>
        /// Units physical attack
        /// </summary>
        public byte Atk;
        
        /// <summary>
        /// Units physical defense
        /// </summary>
        public byte Def;
        
        /// <summary>
        /// Units magical attack
        /// </summary>
        public byte Matk;
        
        /// <summary>
        /// Unit's magical defence
        /// </summary>
        public byte Mdef;
        
        /// <summary>
        /// Unit speed in battle
        /// </summary>
        public byte Speed;
        
        /// <summary>
        /// Unit chance to hit attacks & spells
        /// </summary>
        public byte Accuracy;
        
        /// <summary>
        /// Current unit weight
        /// </summary>
        public byte Weight;
        
        /// <summary>
        /// Unit bonus to how much weight can a party hold in cargo
        /// 10 = 10%
        /// </summary>
        public byte CargoWeightBonusPct;
        
        /// <summary>
        /// Unit current hit points
        /// </summary>
        public byte HP;
        
        /// <summary>
        /// Unit max hit points
        /// </summary>
        public byte MaxHP;
        
        /// <summary>
        /// Units current magic points
        /// </summary>
        public byte MP;
        
        /// <summary>
        /// Units max magic points
        /// </summary>
        public byte MaxMP;

        public UnitStats SetStats(Dictionary<Stat, byte> stats)
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

        public byte GetStat(Stat s)
        {
            return this[s];
        }

        public void SetStat(Stat stat, in byte value)
        {
            this[stat] = value;
        }

        public void AddStat(Stat stat, in byte value)
        {
            this[stat] += value;
        }

        public void SubStat(Stat stat, byte value)
        {
            this[stat] -= value;
        }

        public byte this[Stat stat]
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
                    case Stat.CARGO_WEIGHT: return CargoWeightBonusPct;
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
                    case Stat.DEF: Def = value; break;
                    case Stat.SPEED: Speed = value; break;
                    case Stat.WEIGHT: Weight = value; break;
                    case Stat.MATK: Matk = value; break;
                    case Stat.MDEF: Mdef = value; break;
                    case Stat.ACCURACY: Accuracy = value; break;
                    case Stat.ATK: Atk = value; break;
                    case Stat.CARGO_WEIGHT: CargoWeightBonusPct = value; break;
                    default: throw new Exception("Invalid stat " + stat.ToString());
                }
            }
        }

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
            CargoWeightBonusPct = 0
        };
    }
}

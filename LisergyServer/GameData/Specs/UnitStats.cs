using GameData.buffs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{


    [Serializable]
    public class UnitStats
    {

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

        private ushort _level;

        public void SetStats(UnitStats stats)
        {
            this.HP = stats.HP;
            this.MaxHP = stats.MaxHP;
            this.MP = stats.MP;
            this.MaxMP = stats.MaxMP;
            this.Atk = stats.Atk;
            this.Def = stats.Def;
            this.Speed = stats.Speed;
            this.Accuracy = stats.Accuracy;
            this.Move = stats.Move;
            this.Matk = stats.Matk;
            this.Mdef = stats.Mdef;
            this.Weight = stats.Weight;
        }

        public void SetStats(Dictionary<Stat, ushort> stats)
        {
            foreach (var kp in stats)
                this[kp.Key] = kp.Value;
        }

        public ushort GetStat(Stat s)
        {
            return this[s];
        }

        public void SetStat(Stat stat, ushort value)
        {
            this[stat] = value;
        }

        public UnitStats()
        {
            DefaultValues();
        }

        public UnitStats(Dictionary<Stat, ushort> stats)
        {
            DefaultValues();
            SetStats(stats);
        }

        private void DefaultValues()
        {
            _level = 1;
            Atk = 1;
            Def = 1;
            Matk = 1;
            Mdef = 1;
            Speed = 10;
            Accuracy = 10;
            Weight = 60;

            MaxHP = 5;
            HP = 5;
            MP = 2;
            MaxMP = 2;
            Move = 5;
        }

        // Stats
        public byte Atk { get; set; }
        public byte Def { get; set; }
        public byte Matk { get; set; }
        public byte Mdef { get; set; }
        public byte Speed { get; set; }
        public byte Accuracy { get; set; }
        public byte Weight { get; set; }
        public byte Move { get; set; }

        // Vitals
        public ushort HP { get; set; }
        public ushort MaxHP { get; set; }
        public ushort MP { get; set; }
        public ushort MaxMP { get; set; }
    }
}

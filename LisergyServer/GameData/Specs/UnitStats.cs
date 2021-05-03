using GameData.buffs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{
    [Serializable]
    public class UnitStats
    {
        private Dictionary<Stat, short> _stats = new Dictionary<Stat, short>();
        private int _level;

        public void SetStats(UnitStats stats)
        {
            SetStats(stats._stats);
        }

        public void SetStats(Dictionary<Stat, short> stats)
        {
            foreach (var kp in stats)
                _stats[kp.Key] = kp.Value;
        }

        public short GetStat(Stat s)
        {
            return _stats[s];
        }

        public void SetStat(Stat stat, short value)
        {
            _stats[stat] = value;
        }

        public UnitStats()
        {
            DefaultValues();
        }

        public UnitStats(Dictionary<Stat, short> stats)
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
            Mp = 2;
            MaxMP = 2;
            Move = 5;
        }

        // Stats
        public short Atk { get => _stats[Stat.ATK]; set => _stats[Stat.ATK] = value; }
        public short Def { get => _stats[Stat.DEF]; set => _stats[Stat.DEF] = value; }
        public short Matk { get => _stats[Stat.MATK]; set => _stats[Stat.MATK] = value; }
        public short Mdef { get => _stats[Stat.MDEF]; set => _stats[Stat.MDEF] = value; }
        public short Speed { get => _stats[Stat.SPEED]; set => _stats[Stat.SPEED] = value; }
        public short Accuracy { get => _stats[Stat.ACCURACY]; set => _stats[Stat.ACCURACY] = value; }
        public short Weight { get => _stats[Stat.WEIGHT]; set => _stats[Stat.WEIGHT] = value; }
        public short Move { get => _stats[Stat.MOVE]; set => _stats[Stat.MOVE] = value; }

        // Vitals
        public short HP { get => _stats[Stat.HP]; set => _stats[Stat.HP] = value; }
        public short MaxHP { get => _stats[Stat.MHP]; set => _stats[Stat.MHP] = value; }
        public short Mp { get => _stats[Stat.MP]; set => _stats[Stat.MP] = value; }
        public short MaxMP { get => _stats[Stat.MMP]; set => _stats[Stat.MMP] = value; }
    }
}

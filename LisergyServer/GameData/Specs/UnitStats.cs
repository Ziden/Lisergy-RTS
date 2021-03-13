using GameData.buffs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{
    [Serializable]
    public class UnitStats
    {
        private Dictionary<Stat, ushort> _stats = new Dictionary<Stat, ushort>();
        private int _level;

        public void SetStats(UnitStats stats)
        {
            SetStats(stats._stats);
        }

        public void SetStats(Dictionary<Stat, ushort> stats)
        {
            foreach (var kp in stats)
                _stats[kp.Key] = kp.Value;
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

            MaxHP = 5;
            HP = 5;
            Mp = 2;
            MaxMP = 2;
        }

        // Stats
        public ushort Atk { get => _stats[Stat.ATK]; set => _stats[Stat.ATK] = value; }
        public ushort Def { get => _stats[Stat.DEF]; set => _stats[Stat.DEF] = value; }
        public ushort Matk { get => _stats[Stat.MATK]; set => _stats[Stat.MATK] = value; }
        public ushort Mdef { get => _stats[Stat.MDEF]; set => _stats[Stat.MDEF] = value; }
        public ushort Speed { get => _stats[Stat.SPEED]; set => _stats[Stat.SPEED] = value; }
        public ushort Accuracy { get => _stats[Stat.ACCURACY]; set => _stats[Stat.ACCURACY] = value; }

        // Vitals
        public ushort HP { get => _stats[Stat.HP]; set => _stats[Stat.HP] = value; }
        public ushort MaxHP { get => _stats[Stat.MHP]; set => _stats[Stat.MHP] = value; }
        public ushort Mp { get => _stats[Stat.MP]; set => _stats[Stat.MP] = value; }
        public ushort MaxMP { get => _stats[Stat.MMP]; set => _stats[Stat.MMP] = value; }
    }
}

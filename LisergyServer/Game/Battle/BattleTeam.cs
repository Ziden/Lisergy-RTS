using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Battle
{
    public class BattleTeam
    {
        public List<BattleUnit> Units = new List<BattleUnit>();

        public BattleTeam(params Unit[] units)
        {
            foreach (var unit in units)
                Units.Add(new BattleUnit(this, unit));
        }

        public bool AllDead { get => !Units.Any(u => !u.Dead); }

        public BattleUnit Random()
        {
            return Units.RandomElement();
        }

        public override string ToString()
        {
            return string.Join(",", Units.Select(u => u.ToString()).ToArray());
        }
    }
}

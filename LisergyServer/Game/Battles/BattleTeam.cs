using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battles
{
    [Serializable]
    public class BattleTeam
    {
        public BattleUnit[] Units;

        public BattleTeam(params Unit[] units)
        {
            Units = new BattleUnit[units.Count()];

            for (var x = 0; x < units.Count(); x++)
                Units[x] = new BattleUnit(this, units[x]);
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

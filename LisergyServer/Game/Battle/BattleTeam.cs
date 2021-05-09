using Game.World;
using System;
using System.Linq;

namespace Game.Battles
{
    [Serializable]
    public class BattleTeam
    {
        public BattleUnit[] Units;
        public string OwnerID;
        public bool NPC = false;

        public BattleTeam(params Unit[] units)
        {
            Init(null, units);
        }

        public BattleTeam(PlayerEntity owner, params Unit[] units)
        {
            Init(owner, units);
        }

        private void Init(PlayerEntity owner, params Unit[] units)
        {
            var filtered = units.Where(u => u != null).ToList();
            Units = new BattleUnit[filtered.Count()];
            var isUnitsControlled = owner != null && owner.Online();
            for (var x = 0; x < filtered.Count(); x++)
            {
                Units[x] = new BattleUnit(owner, this, filtered[x]);
                Units[x].Controlled = isUnitsControlled;
            }
            if (owner != null)
            {
                OwnerID = owner.UserID;
            }
        }

        public bool AllDead { get => !Units.Any(u => !u.Dead); }

        public BattleUnit RandomUnit()
        {
            return Units.RandomElement();
        }

        public override string ToString()
        {
            return $"<Team Owner={OwnerID} Units={string.Join(",", Units.Select(u => u.ToString()).ToArray())}";
        }
    }
}

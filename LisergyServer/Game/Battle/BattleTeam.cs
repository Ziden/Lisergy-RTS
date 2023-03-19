using Game.DataTypes;
using Game.Entity.Logic;
using Game.World;
using System;
using System.Linq;

namespace Game.Battles
{
    [Serializable]
    public class BattleTeam
    {
        public BattleUnit[] Units;
        public GameId OwnerID;

        [NonSerialized]
        private IBattleableEntity _entity;

        public BattleTeam(IBattleableEntity entity, params Unit[] units)
        {
            Init(entity, units);
        }

        public BattleTeam(params Unit[] units)
        {
            Init(null, units);
        }

        private void Init(IBattleableEntity entity, params Unit[] units)
        {
            this._entity = entity;
            var owner = entity?.Owner;
            var filtered = units.Where(u => u != null).ToList();
            Units = new BattleUnit[filtered.Count()];
            // All battles are autobattles for now
            var isUnitsControlled = false; // owner != null && owner.Online();
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
        public IBattleableEntity Entity { get => _entity; }

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

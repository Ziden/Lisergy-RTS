using Game.Battler;
using Game.DataTypes;
using Game.World;
using System;
using System.Linq;

namespace Game.Battle
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
            _entity = entity;
            Player.PlayerEntity owner = entity?.Owner;
            System.Collections.Generic.List<Unit> filtered = units.Where(u => u != null).ToList();
            Units = new BattleUnit[filtered.Count()];
            // All battles are autobattles for now
            bool isUnitsControlled = false; // owner != null && owner.Online();
            for (int x = 0; x < filtered.Count(); x++)
            {
                Units[x] = new BattleUnit(owner, this, filtered[x])
                {
                    Controlled = isUnitsControlled
                };
            }
            if (owner != null)
            {
                OwnerID = owner.UserID;
            }
        }

        public bool AllDead => !Units.Any(u => !u.Dead);
        public IBattleableEntity Entity => _entity;

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

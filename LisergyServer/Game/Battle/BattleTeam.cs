using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Player;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battle
{
    [Serializable]
    public class BattleTeam
    {
        public BattleUnit[] Units;
        public GameId OwnerID;
        public bool IsAutoBattle = true;

        [NonSerialized]
        private IEntity _entity;

        public BattleTeam(IEntity entity, params Unit[] units)
        {
            Init(entity, units);
        }

        public BattleTeam(IEntity entity)
        {
            Init(entity, entity.Get<BattleGroupComponent>().Units.ToArray());
        }

        public BattleTeam(IEntity entity, BattleGroupComponent component)
        {
            Init(entity, component.Units.ToArray());
        }

        public BattleTeam(params Unit[] units)
        {
            Init(null, units);
        }

        public BattleUnit Leader => Units[0];

        private void Init(IEntity entity, params Unit[] units)
        {
            _entity = entity;
            PlayerEntity owner = entity?.Owner;
            List<Unit> filtered = units.Where(u => u != null).ToList();
            Units = new BattleUnit[filtered.Count()];
            for (int x = 0; x < filtered.Count(); x++)
            {
                Units[x] = new BattleUnit(owner, this, filtered[x]);
            }
            if (owner != null)
            {
                OwnerID = owner.UserID;
            }
        }

        public BattleUnit[] Alive => Units.Where(u => !u.Dead).ToArray();

        public bool AllDead => !Units.Any(u => !u.Dead);
        public IEntity Entity => _entity;

        public override string ToString()
        {
            return $"<Team Owner={OwnerID} Units={string.Join(",", Units.Select(u => u.ToString()).ToArray())}";
        }
    }
}

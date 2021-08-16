using Game.Battle;
using Game.Battles;
using Game.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entity
{
    [Serializable]
    public class Dungeon : StaticEntity, IBattleable
    {
        protected List<Unit[]> _battles = new List<Unit[]>();
        private Item[] _rewards;

        public List<Unit[]> Battles { get => _battles; }
        public Item[] Rewards { get => _rewards; set => _rewards = value; }

        public Dungeon(): base(Gaia)
        {

        }

        public bool IsComplete()
        {
            return !_battles.Any(battle => battle.Any(unit => unit.Stats.HP != 0));
        }

        protected override void OnBattleFinished(string battleID)
        {
            if (IsComplete())
            {
                this.Tile = null;
            }
        }

        public Dungeon(params Unit[] fights): base(Gaia)
        {
            this._battles.Add(fights);
        }

        public void AddBattle(params Unit[] units)
        {
            this._battles.Add(units);
        }

        public override string ToString()
        {
            return $"<Dungeon battles={_battles.Count}>";
        }

        public BattleTeam GetBattleTeam()
        {
            var units = this.Battles.First();
            return new BattleTeam(this, units);
        }
    }
}

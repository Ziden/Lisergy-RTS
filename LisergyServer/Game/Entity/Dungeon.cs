using Game.Battles;
using Game.Inventories;
using System;

namespace Game.Entity
{
    [Serializable]
    public class Dungeon : WorldEntity
    {
        protected BattleTeam[] _battles;
        private Item[] _rewards;

        public BattleTeam[] Battles { get => _battles; }
        public Item[] Rewards { get => _rewards; set => _rewards = value; }

        public Dungeon(): base(Gaia)
        {

        }

        public Dungeon(params BattleTeam [] fights): base(Gaia)
        {
            this._battles = fights;
        }

        public void SetBattles(params BattleTeam [] battles)
        {
            this._battles = battles;
        }

        public override string ToString()
        {
            return $"<Dungeon battles={_battles.Length}>";
        }
    }
}

using Game.Battles;
using System;

namespace Game.Entity
{

    [Serializable]
    public class Dungeon : WorldEntity
    {
        public BattleTeam[] Battles { get => _battles; }

        protected BattleTeam [] _battles;

        public Dungeon(): base(Gaia)
        {

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

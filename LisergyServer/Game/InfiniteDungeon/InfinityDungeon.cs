using Game.Battles;
using Game.Events;
using Game.Events.ServerEvents;
using System;

namespace Game.InfiniteDungeon
{
    public class InfinityDungeon
    {
        private PlayerEntity _player;

        public string BattleID;

        public Unit[] Units { get; private set; }

        public int CurrentLevel { get; private set; }

        public InfinityDungeon(PlayerEntity player, Unit [] units)
        {
            this.Units = units;
            this.CurrentLevel = 1;
            _player = player;
        }

        public BattleTeam GetNextEnemy()
        {
            return new BattleTeam(null, new Unit()
            {
                Sprite = "thief",
                Name = "Rougue A",
            }, new Unit()
            {
                Sprite = "thief",
                Name = "Rougue B",
            });
        }
    }
}

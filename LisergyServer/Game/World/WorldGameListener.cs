using Game.Battle;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using System;

namespace Game.World
{
    public class WorldGameListener : IEventListener
    {
        private StrategyGame _game;

        public WorldGameListener(StrategyGame game)
        {
            _game = game;
        }

        [EventMethod]
        public void OnOffensiveAction(OffensiveMoveEvent ev)
        {
            IBattleable atk = ev.Attacker as IBattleable;
            IBattleable def = ev.Defender as IBattleable;   
            if(atk != null && def != null)
            {
                var battleID = Guid.NewGuid().ToString();
                ev.Attacker.BattleID = battleID;
                ev.Defender.BattleID = battleID;
                _game.NetworkEvents.RunCallbacks(new BattleStartEvent()
                {
                    X = ev.Defender.X,
                    Y = ev.Defender.Y,
                    Attacker = atk.ToBattleTeam(),
                    Defender = def.ToBattleTeam(),
                    BattleID = battleID
                });
            }
        }
    }
}

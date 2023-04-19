using Game.Network.ServerPackets;
using Game.Scheduler;
using System;
using System.Linq;

namespace Game.Battle
{
    public class BattleTask : GameTask
    {
        private readonly TurnBattle _battle;
        private readonly StrategyGame _game;

        public BattleTask(StrategyGame game, TurnBattle battle) : base(TimeSpan.FromSeconds(3), null)
        {
            _battle = battle;
            _game = game;
        }

        public override void Execute()
        {
            Repeat = false;
            _battle.Task = null;

            var unitRef = _battle.Attacker.Units.First().UnitReference;

            TurnBattleRecord result = _battle.AutoRun.RunAllRounds();
            BattleResultPacket resultEvent = new BattleResultPacket(_battle.ID, result);
            // for now just run callbacks
            // TODO: place on a message queue
            _game.NetworkEvents.Call(resultEvent);
        }

        public override string ToString()
        {
            return $"<BattleTask {ID} {_battle}>";
        }
    }
}

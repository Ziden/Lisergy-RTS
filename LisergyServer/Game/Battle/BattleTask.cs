using Game.Battles;
using Game.Entity;
using Game.Events;
using Game.Scheduler;
using System;
using System.Collections.Generic;

namespace Game.Battles
{
    public class BattleTask : GameTask
    {
        private TurnBattle _battle;
        private BlockchainGame _game;

        public BattleTask(BlockchainGame game, TurnBattle battle) : base(TimeSpan.FromSeconds(3))
        {
            this._battle = battle;
            _game = game;
        }

        public override void Execute()
        {
            Repeat = false;
            //_battle.Task = null;
            var result = _battle.AutoRun.RunAllRounds();
            var resultEvent = new BattleResultPacket(_battle.ID.ToString(), result);
            // for now just run callbacks
            // TODO: place on a message queue
            _game.NetworkEvents.Call(resultEvent);
        }

        public override string ToString()
        {
            return $"<BattleTask {ID.ToString()} {_battle}>";
        }
    }
}

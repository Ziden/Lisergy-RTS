using Game.Network.ServerPackets;
using Game.Scheduler;
using System;

namespace Game.Battle
{
    public class BattleTask : GameTask
    {
        private readonly TurnBattle _battle;
        private readonly GameLogic _game;

        public BattleTask(GameLogic game, TurnBattle battle) : base(TimeSpan.FromSeconds(3), null)
        {
            _battle = battle;
            _game = game;
        }

        public override void Tick()
        {
            Repeat = false;
            _battle.Task = null;

            TurnBattleRecord result = _battle.AutoRun.RunAllRounds();
            BattleResultPacket resultEvent = new BattleResultPacket(_battle.ID, result);
            // for now just run callbacks
            // TODO: place on a message queue
            _game.NetworkPackets.Call(resultEvent);
        }

        public override string ToString()
        {
            return $"<BattleTask {ID} {_battle}>";
        }
    }
}

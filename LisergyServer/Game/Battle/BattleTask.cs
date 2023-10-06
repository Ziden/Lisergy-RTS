using Game.Network.ServerPackets;
using Game.Scheduler;
using System;

namespace Game.Battle
{
    public class BattleTask : GameTask
    {
        private readonly TurnBattle _battle;
        private readonly IGame _game;

        public BattleTask(IGame game, TurnBattle battle) : base(game, TimeSpan.FromSeconds(3), null)
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
            _game.Network.IncomingPackets.Call(resultEvent);
        }

        public override string ToString()
        {
            return $"<BattleTask {ID} {_battle}>";
        }
    }
}

using Game.Battle.BattleActions;
using Game.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battle
{
    public class AutoRun
    {

        private readonly TurnBattle _battle;
        private readonly GameRandom _random;

        public AutoRun(TurnBattle battle)
        {
            _battle = battle;
            _random = new GameRandom(battle.ID);
        }

        private TurnBattleRecord Log => _battle.Result;

        private SortedSet<BattleUnit> UnitQueue => _battle._actionQueue;

        private bool IsOver =>_battle.Attacker.AllDead || _battle. Defender.AllDead;

        public TurnBattleRecord RunAllRounds()
        {
            while (!IsOver)
            {
                _ = RunOneTurn();
            }
            Log.Winner = _battle.Attacker.AllDead ? _battle.Defender : _battle.Attacker;
            return Log;
        }

        public virtual List<BattleEvent> RunOneTurn()
        {
            BattleUnit actingUnit = UnitQueue.First();
            return TakeAction(actingUnit);
        }

        protected virtual List<BattleEvent> TakeAction(BattleUnit unit)
        {
            BattleTeam enemyTeam = _battle.GetOpposingTeam(unit);
            BattleUnit enemy = GetRandomTarget(enemyTeam);
            AttackAction action = new AttackAction(_battle, unit, enemy);
            return _battle.ReceiveAction(action);
        }

        private BattleUnit GetRandomTarget(BattleTeam team)
        {
            var alive = team.Alive;
            return alive[_random.Next(alive.Length)];
        }
    }
}

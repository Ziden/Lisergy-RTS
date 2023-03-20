using Game.BattleActions;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battle
{
    public class AutoRun
    {

        private readonly TurnBattle _battle;

        public AutoRun(TurnBattle battle)
        {
            _battle = battle;
        }

        private TurnBattleResult Log => _battle.Result;

        private SortedSet<BattleUnit> UnitQueue => _battle._actionQueue;

        public TurnBattleResult RunAllRounds()
        {
            while (!_battle.IsOver)
            {
                _ = PlayOneTurn();
            }
            Log.Winner = _battle.Attacker.AllDead ? _battle.Defender : _battle.Attacker;
            return Log;
        }

        public virtual List<BattleAction> PlayOneTurn()
        {
            Log.NextTurn();
            BattleUnit actingUnit = UnitQueue.First();
            return TakeAction(actingUnit);
        }

        protected virtual List<BattleAction> TakeAction(BattleUnit unit)
        {
            BattleTeam enemyTeam = _battle.GetOpposingTeam(unit);
            BattleUnit enemy = enemyTeam.RandomUnit();
            AttackAction action = new AttackAction(_battle, unit, enemy);
            return _battle.ReceiveAction(action);
        }


    }
}


using Game.Battles.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battles
{
    public class AutoRun
    {

        private TurnBattle _battle;

        public AutoRun(TurnBattle battle)
        {
            this._battle = battle;
        }

        TurnBattleResult Log => _battle.Result;
        SortedSet<BattleUnit> UnitQueue => _battle._actionQueue;

        public TurnBattleResult RunAllRounds()
        {
            while (!_battle.IsOver) {
                PlayOneTurn();
            }
            Log.Winner = _battle.Attacker.AllDead ? _battle.Defender : _battle.Attacker;
            return Log;
        }

        public virtual List<BattleAction> PlayOneTurn()
        {
            Log.NextTurn();
            var actingUnit = UnitQueue.First();
            return TakeAction(actingUnit);
        }

        protected virtual List<BattleAction> TakeAction(BattleUnit unit)
        {
            var enemyTeam = _battle.GetOpposingTeam(unit);
            var enemy = enemyTeam.RandomUnit();
            var action = new AttackAction(_battle, unit, enemy);
            return _battle.ReceiveAction(action);     
        }

        
    }
}

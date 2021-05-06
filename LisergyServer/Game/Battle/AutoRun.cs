
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
            while (!_battle.Attacker.AllDead && !_battle.Defender.AllDead) {
                PlayOneTurn();
            }
            Log.Winner = _battle.Attacker.AllDead ? _battle.Defender : _battle.Attacker;
            return Log;
        }

        public virtual BattleAction PlayOneTurn()
        {
            Log.NextTurn();
            var actingUnit = UnitQueue.First();
            TakeAction(actingUnit);
            if (!UnitQueue.Remove(actingUnit))
                throw new Exception($"Error removing {actingUnit} from the UnitQueue");
            actingUnit.RT += actingUnit.GetMaxRT();
            UnitQueue.Add(actingUnit);
            return Log.LastAction;
        }

        protected virtual void TakeAction(BattleUnit unit)
        {
            var enemyTeam = _battle.GetOpposingTeam(unit);
            var enemy = enemyTeam.RandomUnit();
            var action = new AttackAction(unit, enemy);
            _battle.ReceiveAction(action);     
        }

        
    }
}

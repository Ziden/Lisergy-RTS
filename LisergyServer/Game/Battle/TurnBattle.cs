
using Game.Battles.Actions;
using Game.BattleTactics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battles
{
    public class TurnBattle
    {
        internal SortedSet<BattleUnit> _actionQueue = new SortedSet<BattleUnit>();
        internal TurnBattleResult Result = new TurnBattleResult();

        public Guid ID { get; private set; }
        public BattleTeam Attacker { get; private set; }
        public BattleTeam Defender { get; private set; }
        public AutoRun AutoRun { get; set; }

        public BattleUnit CurrentActingUnit => _actionQueue.First();

        public TurnBattle(Guid id, BattleTeam attacker, BattleTeam defender)
        {
            this.ID = id;
            Attacker = Result.Attacker = attacker;
            Defender = Result.Defender = defender;
            AutoRun = new AutoRun(this);

            _actionQueue.UnionWith(attacker.Units);
            _actionQueue.UnionWith(defender.Units);
        }

        public void ReceiveAction(BattleAction action)
        {
            Result.NextTurn();
            if (CurrentActingUnit != action.Unit)
            {
                action.Result = new ActionResult();
                action.Result.Succeeded = false;
                return;
            }

            if(action is AttackAction)
            {
                var attack = (AttackAction)action;
                action.Result = attack.Unit.Attack(attack.Defender);
                action.Result.Succeeded = true;   
            }
            Result.AddAction(action);
        }

        public virtual BattleTeam GetOpposingTeam(BattleUnit unit)
        {
            if (unit.Team == Attacker) return Defender;
            return Attacker;
        }
    }
}

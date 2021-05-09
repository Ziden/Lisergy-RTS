
using Game.Battles.Actions;
using Game.BattleTactics;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battles
{
    public class TurnBattle
    {
        internal SortedSet<BattleUnit> _actionQueue = new SortedSet<BattleUnit>();
        internal TurnBattleResult Result = new TurnBattleResult();

        public BattleStartEvent StartEvent;

        public Guid ID { get; private set; }
        public BattleTeam Attacker { get; private set; }
        public BattleTeam Defender { get; private set; }
        public AutoRun AutoRun { get; set; }

        public BattleUnit CurrentActingUnit => _actionQueue.First();

        public bool IsOver => Attacker.AllDead || Defender.AllDead;

        public TurnBattle(Guid id, BattleTeam attacker, BattleTeam defender)
        {
            this.ID = id;
            Attacker = Result.Attacker = attacker;
            Defender = Result.Defender = defender;
            AutoRun = new AutoRun(this);

            _actionQueue.UnionWith(attacker.Units);
            _actionQueue.UnionWith(defender.Units);
        }

        public List<BattleAction> ReceiveAction(BattleAction action)
        {
            Result.NextTurn();
            var unit = CurrentActingUnit;
            if (unit != action.Unit)
            {
                action.Result = new ActionResult();
                action.Result.Succeeded = false;
                return null;
            }
            if(action is AttackAction)
            {
                var attack = (AttackAction)action;
                action.Result = attack.Unit.Attack(attack.Defender);
                action.Result.Succeeded = true;   
            }
            UpdateRT(unit);
            Result.AddAction(action);
            return Result.CurrentTurn.Actions;
        }

        public void UpdateRT(BattleUnit unit)
        {
            _actionQueue.Remove(unit);
            unit.RT += unit.GetMaxRT();
            _actionQueue.Add(unit);
        }

        public virtual BattleTeam GetOpposingTeam(BattleUnit unit)
        {
            if (unit.Team == Attacker) return Defender;
            return Attacker;
        }

        public override string ToString()
        {
            return $"<Battle ID={ID} Atk={Attacker} Def={Defender}/>";
        }

        public Unit FindUnit(string id)
        {
            var unit = Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            if (unit == null) unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            return unit.UnitReference;
        }

        public BattleUnit FindBattleUnit(string id)
        {
            var unit = Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            if (unit == null) unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            return unit;
        }
    }
}

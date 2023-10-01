using Game.BattleActions;
using Game.BattleEvents;
using Game.Battler;
using Game.DataTypes;
using Game.Events;
using Game.Network.ServerPackets;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battle
{
    public class TurnBattle
    {
        public BattleTask Task;
        internal SortedSet<BattleUnit> _actionQueue = new SortedSet<BattleUnit>();
        public TurnBattleRecord Result = new TurnBattleRecord();

        public BattleStartPacket StartEvent;

        public GameId ID { get; private set; }
        public BattleTeam Attacker { get; private set; }
        public BattleTeam Defender { get; private set; }
        public AutoRun AutoRun { get; set; }

        public BattleUnit CurrentActingUnit => _actionQueue.First();

        public bool IsOver => Attacker.AllDead || Defender.AllDead;

        public TurnBattle(GameId id, BattleTeam attacker, BattleTeam defender)
        {
            ID = id;
            Attacker = Result.Attacker = attacker;
            Defender = Result.Defender = defender;
            AutoRun = new AutoRun(this);

            _actionQueue.UnionWith(attacker.Units);
            _actionQueue.UnionWith(defender.Units);
        }

        public void Death(BattleUnit u)
        {
            Result.RecordEvent(new UnitDeadEvent() { UnitId = u.UnitID });
            _actionQueue.Remove(u);
        }

        public List<BattleEvent> ReceiveAction(BattleAction action)
        {
            Result.NextTurn();
            BattleUnit unit = CurrentActingUnit;
            if (unit != action.Unit)
            {
                return null;
            }
            if (action is AttackAction attack)
            {
                attack.Result = attack.Unit.Attack(attack.Defender);
                attack.Result.Succeeded = true;
                Result.RecordEvent(attack);
                if (attack.Defender.Dead)
                {
                    Death(attack.Defender);
                }
            }
            UpdateRT(unit);
            return Result.CurrentTurn.Events;
        }

        public void UpdateRT(BattleUnit unit)
        {
            _ = _actionQueue.Remove(unit);
            unit.RT += unit.GetMaxRT();
            _ = _actionQueue.Add(unit);
        }

        public virtual BattleTeam GetOpposingTeam(BattleUnit unit)
        {
            return unit.Team == Attacker ? Defender : Attacker;
        }

        public override string ToString()
        {
            return $"<Battle ID={ID} Atk={Attacker} Def={Defender}/>";
        }

        public Unit FindUnit(GameId id)
        {
            BattleUnit unit = Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            if (unit == null)
            {
                unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            }

            return unit.UnitReference;
        }

        public BattleUnit FindBattleUnit(GameId id)
        {
            BattleUnit unit = Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            if (unit == null)
            {
                unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            }

            return unit;
        }
    }
}

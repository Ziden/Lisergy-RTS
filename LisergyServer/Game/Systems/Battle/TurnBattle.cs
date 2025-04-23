using System.Collections.Generic;
using System.Linq;
using Game.Engine.DataTypes;
using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.BattleEvents;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;

namespace Game.Systems.Battle
{
	public class TurnBattle
	{
		internal SortedSet<BattleUnit> _actionQueue = new SortedSet<BattleUnit>();
		public TurnBattleRecord Record = new TurnBattleRecord();

		public TurnBattle(GameId id, in BattleGroupData attacker, in BattleGroupData defender)
		{
			ID = id;
			Attacker = Record.Attacker = new BattleTeam(attacker);
			Defender = Record.Defender = new BattleTeam(defender);
			AutoRun = new AutoRun(this);

			_actionQueue.UnionWith(Attacker.Units);
			_actionQueue.UnionWith(Defender.Units);
		}

		public GameId ID { get; }
		public BattleTeam Attacker { get; }
		public BattleTeam Defender { get; }
		public AutoRun AutoRun { get; set; }
		public BattleUnit CurrentActingUnit => _actionQueue.First();
		public bool IsOver { get; set; }

		private void FinishBattle()
		{
			IsOver = true;
			Attacker.UpdateTeamData();
			Defender.UpdateTeamData();
		}

		public void Death(BattleUnit u)
		{
			Record.RecordEvent(new UnitDeadEvent {UnitId = u.UnitID});
			_actionQueue.Remove(u);
			if (u.Team.AllDead) FinishBattle();
		}

		public List<BattleEvent> ReceiveAction(BattleAction action)
		{
			Record.NextTurn();
			var unit = CurrentActingUnit;
			if (unit != action.Unit) return null;
			if (action is AttackAction attack)
			{
				attack.Result = attack.Unit.Attack(attack.Defender);
				attack.Result.Succeeded = true;
				Record.RecordEvent(attack);
				if (attack.Defender.Dead) Death(attack.Defender);
			}

			UpdateRT(unit);

			return Record.CurrentTurn.Events;
		}

		public void UpdateRT(BattleUnit unit)
		{
			_ = _actionQueue.Remove(unit);
			unit.RT += unit.MaxRT;
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
			var unit = Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
			if (unit == null) unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);
			return unit.UnitData;
		}

		public BattleUnit FindBattleUnit(GameId id)
		{
			var unit = Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
			if (unit == null) unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);

			return unit;
		}
	}
}
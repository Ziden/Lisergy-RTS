using System;
using Game.Engine.DataTypes;
using Game.Systems.Battle.Data;

namespace Game.Systems.Battle.BattleActions
{
	[Serializable]
	public class BattleAction : BattleEvent
	{
		private ActionResult _result;

		[NonSerialized] private BattleUnit _unit;

		[NonSerialized] public TurnBattle Battle;

		public GameId UnitID;

		public BattleAction(TurnBattle battle, BattleUnit atk)
		{
			Unit = atk;
			Battle = battle;
		}

		public BattleUnit Unit
		{
			get
			{
				if (_unit == null) _unit = Battle.FindBattleUnit(UnitID);

				return _unit;
			}
			set
			{
				_unit = value;
				UnitID = value?.UnitID ?? GameId.ZERO;
			}
		}

		public ActionResult Result
		{
			get => _result;
			set => _result = value;
		}
	}
}
using System;
using Game.Engine.DataTypes;
using Game.Systems.Battle.Data;
using Newtonsoft.Json;

namespace Game.Systems.Battle.BattleActions
{
	[Serializable]
	public class AttackAction : BattleAction
	{
		[NonSerialized] [JsonIgnore] private BattleUnit _defender;

		public GameId DefenderID;

		public AttackAction(TurnBattle battle, BattleUnit atk, BattleUnit def) : base(battle, atk)
		{
			Defender = def;
		}

		public BattleUnit Defender
		{
			get
			{
				if (_defender == null) _defender = Battle.FindBattleUnit(DefenderID);

				return _defender;
			}
			set
			{
				_defender = value;
				DefenderID = value?.UnitID ?? GameId.ZERO;
			}
		}

		public override string ToString()
		{
			return $"<Attack From={UnitID} To={DefenderID}>";
		}
	}
}
using System;
using Game.Systems.Battle.BattleActions;

namespace Game.Systems.Battle
{
	[Serializable]
	public class SerializableResult
	{
		public BattleTeam Attacker;

		public string BattleID;
		public BattleTeam Defender;

		public SerializableTurn[] Turns;

		public SerializableResult(TurnBattleRecord result)
		{
			Attacker = result.Attacker;
			Defender = result.Defender;
			Turns = new SerializableTurn[result.Turns.Count];
			for (var x = 0; x < Turns.Length; x++) Turns[x] = new SerializableTurn(result.Turns[x]);
		}
	}

	[Serializable]
	public class SerializableTurn
	{
		public BattleEvent[] Events;

		public SerializableTurn(TurnLog turnLog)
		{
			Events = new BattleAction[turnLog.Events.Count];
			for (var x = 0; x < turnLog.Events.Count; x++) Events[x] = turnLog.Events[x];
		}
	}
}
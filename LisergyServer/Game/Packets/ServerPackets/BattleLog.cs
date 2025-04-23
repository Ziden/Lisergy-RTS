using System;
using Game.Engine;
using Game.Engine.Network;
using Game.Systems.Battle;
using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.Data;

namespace Game.Network.ServerPackets
{
    /// <summary>
    ///     Full battle log so it can be replayed.
    /// </summary>
    [Serializable]
	public class BattleLogPacket : BasePacket, IServerPacket
	{
		public byte[] BattleStartHeaderData;
		public BattleTurnLog[] Turns;

		public BattleLogPacket()
		{
		}

		public BattleLogPacket(TurnBattle battle)
		{
			if (battle.IsOver || battle.Record.Turns.Count > 0)
				throw new Exception("Cannot start a battle log from a battle that already started");
			BattleStartHeaderData = Serialization.FromAnyType(new BattleHeader
			{
				BattleID = battle.ID,
				BattleTime = DateTime.UtcNow,
				Attacker = battle.Attacker.GroupData,
				Defender = battle.Defender.GroupData
			}).ToArray();
		}

		public BattleLogPacket(BattleQueuedPacket start)
		{
			BattleStartHeaderData = Serialization.FromAnyType(new BattleHeader
			{
				BattleID = start.BattleID,
				BattleTime = DateTime.UtcNow,
				Attacker = start.Attacker,
				Defender = start.Defender
			}).ToArray();
		}

		public BattleHeader DeserializeStartingState()
		{
			return Serialization.ToAnyType<BattleHeader>(BattleStartHeaderData);
		}

		public void SetTurns(TurnBattleRecord result)
		{
			Turns = new BattleTurnLog[result.Turns.Count];
			for (var x = 0; x < Turns.Length; x++) Turns[x] = new BattleTurnLog(result.Turns[x]);
		}

		public void SetTurns(BattleResultPacket result)
		{
			Turns = new BattleTurnLog[result.Turns.Length];
			for (var x = 0; x < Turns.Length; x++) Turns[x] = new BattleTurnLog(result.Turns[x]);
		}
	}

	[Serializable]
	public class BattleTurnLog
	{
		public BattleEvent[] Events;

		public BattleTurnLog()
		{
		}

		public BattleTurnLog(TurnLog turnLog)
		{
			Events = turnLog.Events.ToArray();
		}

		public BattleTurnLog(BattleTurnLog turnLog)
		{
			Events = turnLog.Events;
		}
	}
}
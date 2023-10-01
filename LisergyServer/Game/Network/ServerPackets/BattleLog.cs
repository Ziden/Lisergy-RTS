using Game.Battle;
using Game.Battle.BattleActions;
using Game.Events;
using System;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Full battle log so it can be replayed.
    /// </summary>
    [Serializable]
    public class BattleLogPacket : ServerPacket
    {
        public byte [] BattleStartHeaderData;

        public CompleteBattleHeader DeserializeStartingState() => Serialization.ToAnyType<CompleteBattleHeader>(BattleStartHeaderData);

        public BattleTurnLog[] Turns;

        public BattleLogPacket(TurnBattle battle)
        {
            if(battle.IsOver || battle.Result.Turns.Count > 0)
            {
                throw new Exception("Cannot start a battle log from a battle that already started");
            }
            BattleStartHeaderData = Serialization.FromAnyType(new CompleteBattleHeader
            {
                BattleID = battle.ID,
                Date = DateTime.UtcNow,
                Attacker = battle.Attacker,
                Defender = battle.Defender
            });
        }

        public BattleLogPacket(BattleStartPacket start)
        {
            BattleStartHeaderData = Serialization.FromAnyType(new CompleteBattleHeader
            {
                BattleID = start.BattleID,
                Date = DateTime.UtcNow,
                Attacker = start.Attacker,
                Defender = start.Defender
            });
        }

        public void SetTurns(TurnBattleRecord result)
        {
            Turns = new BattleTurnLog[result.Turns.Count];
            for (int x = 0; x < Turns.Length; x++)
            {
                Turns[x] = new BattleTurnLog(result.Turns[x]);
            }
        }

        public void SetTurns(BattleResultPacket result)
        {
            Turns = new BattleTurnLog[result.Turns.Length];
            for (int x = 0; x < Turns.Length; x++)
            {
                Turns[x] = new BattleTurnLog(result.Turns[x]);
            }
        }
    }

    [Serializable]
    public class BattleTurnLog
    {
        public BattleEvent[] Events;

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

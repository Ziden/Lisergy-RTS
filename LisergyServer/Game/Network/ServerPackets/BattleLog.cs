using Game.Battle;
using Game.Battle.BattleActions;
using Game.Battle.Data;
using System;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Full battle log so it can be replayed.
    /// </summary>
    [Serializable]
    public class BattleLogPacket : BasePacket, IServerPacket
    {
        public byte [] BattleStartHeaderData;

        public BattleHeaderData DeserializeStartingState() => Serialization.ToAnyType<BattleHeaderData>(BattleStartHeaderData);

        public BattleTurnLog[] Turns;

        public BattleLogPacket(TurnBattle battle)
        {
            if(battle.IsOver || battle.Record.Turns.Count > 0)
            {
                throw new Exception("Cannot start a battle log from a battle that already started");
            }
            BattleStartHeaderData = Serialization.FromAnyType(new BattleHeaderData
            {
                BattleID = battle.ID,
                BattleTime = DateTime.UtcNow,
                Attacker = battle.Attacker.TeamData,
                Defender = battle.Defender.TeamData
            });
        }

        public BattleLogPacket(BattleQueuedPacket start)
        {
            BattleStartHeaderData = Serialization.FromAnyType(new BattleHeaderData
            {
                BattleID = start.BattleID,
                BattleTime = DateTime.UtcNow,
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

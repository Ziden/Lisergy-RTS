using Game.Systems.Battle.BattleActions;
using System;

namespace Game.Systems.Battle
{
    [Serializable]
    public class SerializableResult
    {
        public SerializableResult(TurnBattleRecord result)
        {
            Attacker = result.Attacker;
            Defender = result.Defender;
            Turns = new SerializableTurn[result.Turns.Count];
            for (int x = 0; x < Turns.Length; x++)
            {
                Turns[x] = new SerializableTurn(result.Turns[x]);
            }
        }

        public string BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public SerializableTurn[] Turns;
    }

    [Serializable]
    public class SerializableTurn
    {
        public BattleEvent[] Events;

        public SerializableTurn(TurnLog turnLog)
        {
            Events = new BattleAction[turnLog.Events.Count];
            for (int x = 0; x < turnLog.Events.Count; x++)
            {
                Events[x] = turnLog.Events[x];
            }
        }
    }
}

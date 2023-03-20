using Game.BattleActions;
using System;

namespace Game.Battle
{
    [Serializable]
    public class SerializableResult
    {
        public SerializableResult(TurnBattleResult result)
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
        public BattleAction[] Actions;

        public SerializableTurn(TurnLog turnLog)
        {
            Actions = new BattleAction[turnLog.Actions.Count];
            for (int x = 0; x < turnLog.Actions.Count; x++)
            {
                Actions[x] = turnLog.Actions[x];
            }
        }
    }
}

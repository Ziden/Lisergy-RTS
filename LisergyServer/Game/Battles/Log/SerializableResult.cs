using System;
using System.Text;

namespace Game.Battles.Log
{
    [Serializable]
    public class SerializableResult
    {
        public SerializableResult(BattleResult result)
        {
            Attacker = result.Attacker;
            Defender = result.Defender;
            Turns = new SerializableTurn[result.Turns.Count];
            for (var x = 0; x < Turns.Length; x++)
                Turns[x] = new SerializableTurn(result.Turns[x]);
        }

        public string BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public SerializableTurn[] Turns;
    }

    [Serializable]
    public class SerializableTurn
    {
        public ActionLog[] Actions;

        public SerializableTurn(TurnLog turnLog)
        {
            Actions = new ActionLog[turnLog.Actions.Count];
            for (var x = 0; x < turnLog.Actions.Count; x++)
                Actions[x] = turnLog.Actions[x];
        }
    }
}

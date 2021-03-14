using Game.Battle;
using Game.Battle.Log;
using System;

namespace Game.Events
{
    [Serializable]
    public class BattleResultCompleteEvent : ServerEvent
    {
        public string BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public BattleTurnEvent[] Turns;

        public BattleResultCompleteEvent(BattleResult result)
        {
            Attacker = result.Attacker;
            Defender = result.Defender;
            Turns = new BattleTurnEvent[result.Turns.Count];
            for (var x = 0; x < Turns.Length; x++)
                Turns[x] = new BattleTurnEvent(result.Turns[x]);
        }

        public override EventID GetID() => EventID.BATTLE_RESULT_COMPLETE;
    }

    [Serializable]
    public class BattleTurnEvent
    {
        public ActionLog[] Actions;

        public BattleTurnEvent(TurnLog turnLog)
        {
            Actions = new ActionLog[turnLog.Actions.Count];
            for (var x = 0; x < turnLog.Actions.Count; x++)
                Actions[x] = turnLog.Actions[x];
        }
    }
}

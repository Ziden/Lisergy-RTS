using Game.Battle;
using Game.Battles;
using Game.Battles.Actions;
using System;

namespace Game.Events
{
    [Serializable]
    public class BattleResultEvent : ServerEvent
    {
        public BattleHeader BattleHeader;

        public BattleTurnEvent[] Turns;

        public BattleResultEvent(string battleID, TurnBattleResult result)
        {
            BattleHeader = new BattleHeader();
            BattleHeader.BattleID = battleID;
            BattleHeader.Date = DateTime.UtcNow;
            BattleHeader.Attacker = result.Attacker;
            BattleHeader.Defender = result.Defender;
            Turns = new BattleTurnEvent[result.Turns.Count];
            BattleHeader.AttackerWins = BattleHeader.Attacker == result.Winner;
            for (var x = 0; x < Turns.Length; x++)
                Turns[x] = new BattleTurnEvent(result.Turns[x]);
        }
    }

    [Serializable]
    public class BattleTurnEvent
    {
        public BattleAction[] Actions;

        public BattleTurnEvent(TurnLog turnLog)
        {
            Actions = new BattleAction[turnLog.Actions.Count];
            for (var x = 0; x < turnLog.Actions.Count; x++)
                Actions[x] = turnLog.Actions[x];
        }
    }
}

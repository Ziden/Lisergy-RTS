using Game.Battles;
using System;

namespace Game.Events.ClientEvents
{
    [Serializable]
    public class BattleActionEvent : ClientEvent
    {
        public BattleActionEvent(string BattleID, BattleAction action)
        {
            this.Action = action;
            this.BattleID = BattleID;
        }

        public BattleAction Action;
        public string BattleID;

        public override EventID GetID() => EventID.BATTLE_ACTION;
    }
}

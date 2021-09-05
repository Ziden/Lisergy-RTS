using Game.Battles;
using System;

namespace Game.Events.ClientEvents
{
    [Serializable]
    public class BattleActionPacket : ClientEvent
    {
        public BattleActionPacket(string BattleID, BattleAction action)
        {
            this.Action = action;
            this.BattleID = BattleID;
        }

        public BattleAction Action;
        public string BattleID;
    }
}

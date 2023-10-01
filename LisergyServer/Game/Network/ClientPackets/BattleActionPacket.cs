using Game.Battle.BattleActions;
using Game.Events;
using System;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class BattleActionPacket : ClientPacket
    {
        public BattleActionPacket(string BattleID, BattleAction action)
        {
            Action = action;
            this.BattleID = BattleID;
        }

        public BattleAction Action;
        public string BattleID;
    }
}

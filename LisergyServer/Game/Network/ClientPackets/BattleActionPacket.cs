using Game.Battle.BattleActions;
using System;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class BattleActionPacket : InputPacket
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

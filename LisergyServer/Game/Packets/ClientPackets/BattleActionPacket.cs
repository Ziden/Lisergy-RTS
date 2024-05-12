using Game.Engine.Network;
using Game.Systems.Battle.BattleActions;
using System;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class BattleActionPacket : BasePacket, IClientPacket
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

using Game.Battle.BattleActions;
using Game.Events;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class BattleActionResultPacket : ClientPacket
    {
        public ActionResult ActionResult;
    }
}

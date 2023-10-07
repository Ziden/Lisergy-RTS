using Game.Battle.BattleActions;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class BattleActionResultPacket : InputPacket
    {
        public ActionResult ActionResult;
    }
}

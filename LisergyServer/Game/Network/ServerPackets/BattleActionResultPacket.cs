using Game.Battle.BattleActions;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class BattleActionResultPacket : BasePacket, IServerPacket
    {
        public ActionResult ActionResult;
    }
}

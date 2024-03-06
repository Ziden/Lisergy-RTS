using Game.Engine.Network;
using Game.Systems.Battle.BattleActions;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class BattleActionResultPacket : BasePacket, IServerPacket
    {
        public ActionResult ActionResult;
    }
}

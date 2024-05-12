using System;
using System.Collections.Generic;
using System.Text;
using Game.Engine.Network;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class InvalidSessionPacket : BasePacket, IServerPacket
    {
    }
}

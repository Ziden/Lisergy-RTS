using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class InvalidSessionPacket : BasePacket, IServerPacket
    {
    }
}

using Game.Engine.Network;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class InvalidSessionPacket : BasePacket, IServerPacket
    {
    }
}

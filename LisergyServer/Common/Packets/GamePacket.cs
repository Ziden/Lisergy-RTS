using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Packets
{
    public abstract class GamePacket
    {
        PacketTypes PacketId { get; }
    }
}

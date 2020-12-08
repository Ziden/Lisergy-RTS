using FlatSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Packets
{
    [FlatBufferStruct]
    public class LoginPacket: GamePacket
    {
        public PacketTypes PacketId => PacketTypes.LOGIN;

        [FlatBufferItem(0)]
        public virtual string Login { get; set; }

        [FlatBufferItem(1)]
        public virtual string Password { get; set; }
    }
}

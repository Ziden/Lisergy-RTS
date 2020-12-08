using FlatSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Packets
{
    public class PacketSerializer
    {
        // TODO: Improve this
        public static GamePacket Deserialize(byte [] bytes)
        {
            var id = (PacketTypes)bytes[0];
            switch(id)
            {
                case PacketTypes.LOGIN: return FlatBufferSerializer.Default.Parse<LoginPacket>(bytes);
            }
            return null;
        }
    }
}

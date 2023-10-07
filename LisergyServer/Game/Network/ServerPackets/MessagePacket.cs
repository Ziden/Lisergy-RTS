using System;
using Game.Network;

namespace Game.Events.ServerEvents
{
    public enum MessageType
    {
        BAD_INPUT = 1,
        RAW_TEXT = 2
    }

    [Serializable]
    public class MessagePacket : ServerPacket
    {
        public MessagePacket(MessageType type, string args)
        {
            Type = type;
            Args = args;
        }

        public string Args;
        public MessageType Type;
    }
}

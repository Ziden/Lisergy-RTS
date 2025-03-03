﻿using Game.Engine.Network;
using System;

namespace Game.Events.ServerEvents
{
    public enum MessageType
    {
        BAD_INPUT = 1,
        RAW_TEXT = 2
    }

    [Serializable]
    public class MessagePacket : BasePacket, IServerPacket
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

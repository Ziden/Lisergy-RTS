using Game.Engine.DataTypes;
using Game.Engine.Network;
using System;
using System.Collections.Generic;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class ChatPacket : BasePacket, IClientPacket
    {
        public GameId Owner;
        public string Name;
        public string Message;
        public DateTime Time;
    }

    [Serializable]
    public class ChatLogPacket : BasePacket, IServerPacket
    {
        public List<ChatPacket> Messages;
    }
}

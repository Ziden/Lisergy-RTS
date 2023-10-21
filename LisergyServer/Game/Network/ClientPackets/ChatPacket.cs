using Game.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

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

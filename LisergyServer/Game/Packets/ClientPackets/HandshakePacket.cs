﻿using Game.Engine.DataTypes;
using Game.Engine.Network;
using System;

namespace Game.Network.ClientPackets
{
    /// <summary>
    /// Send by a client to services to authenticate in services using a token
    /// generated by account service
    /// </summary>
    [Serializable]
    public class HandshakePacket : BasePacket, IClientPacket
    {
        public GameId PlayerId;
        public string Token;
    }
}

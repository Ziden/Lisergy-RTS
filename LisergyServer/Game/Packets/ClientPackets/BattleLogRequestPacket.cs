﻿using Game.Engine.DataTypes;
using Game.Engine.Network;
using System;

namespace Game.Network.ClientPackets
{
    /// <summary>
    /// When client requests a full log to either view the battle log or 
    /// simulate the battle on client side to display it
    /// </summary>
    [Serializable]
    public class BattleLogRequestPacket : BasePacket, IClientPacket
    {
        public GameId BattleId;

        public void Execute(IGame game)
        {
            throw new NotImplementedException();
        }
    }

}

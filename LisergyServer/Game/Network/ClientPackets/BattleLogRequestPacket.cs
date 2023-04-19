using Game.DataTypes;
using Game.Events;
using System;

namespace Game.Network.ClientPackets
{
    /// <summary>
    /// When client requests a full log to either view the battle log or 
    /// simulate the battle on client side to display it
    /// </summary>
    [Serializable]
    public class BattleLogRequestPacket : ClientPacket
    {
        public GameId BattleId;
    }

}

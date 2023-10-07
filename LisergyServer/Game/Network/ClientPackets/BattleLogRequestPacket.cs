using Game.DataTypes;
using System;

namespace Game.Network.ClientPackets
{
    /// <summary>
    /// When client requests a full log to either view the battle log or 
    /// simulate the battle on client side to display it
    /// </summary>
    [Serializable]
    public class BattleLogRequestPacket : InputPacket
    {
        public GameId BattleId;
    }

}

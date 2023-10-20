using Game.DataTypes;
using Game.Network;

namespace BaseServer.Core
{
    public class RoutedPacket : BasePacket
    {
        byte[] Payload; 
        GameId Destination;
    }
}

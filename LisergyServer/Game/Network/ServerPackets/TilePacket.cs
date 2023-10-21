using Game.Network;
using Game.Systems.Tile;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TilePacket : BasePacket, IServerPacket
    {
        public TileMapData Data;

        public override string ToString()
        {
            return $"<TilePacket {Data}>";
        }
    }
}

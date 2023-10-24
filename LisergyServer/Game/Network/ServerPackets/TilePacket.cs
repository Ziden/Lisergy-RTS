using Game.Network;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TilePacket : BasePacket, IServerPacket
    {
        public TileData Data;
        public Position Position;

        public override string ToString()
        {
            return $"<TilePacket {Data}>";
        }
    }
}

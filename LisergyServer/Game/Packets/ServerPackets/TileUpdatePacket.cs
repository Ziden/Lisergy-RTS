using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TileUpdatePacket : ServerPacket
    {
        public TileUpdatePacket(Tile tile)
        {
            this.Tile = tile;
        }

        public Tile Tile;

        public override string ToString()
        {
            return $"<TileUpdate {Tile}>";
        }
    }
}

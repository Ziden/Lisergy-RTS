using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TileVisibleEvent : ServerEvent
    {
        public TileVisibleEvent(Tile tile)
        {
            this.Tile = tile;
        }

        public Tile Tile;

        public override string ToString()
        {
            return $"<TileVisible {Tile}>";
        }
    }
}

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

        [NonSerialized]
        private WorldEntity _viewer;

        public WorldEntity Viewer
        {
            get
            {
                return _viewer;
            }
            set {
                _viewer = value;
            }
        }

        public override EventID GetID() => EventID.TILE_VISIBLE;
    }
}

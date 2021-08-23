using Game.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class PlayerVisibilityChangeEvent : GameEvent
    {
        public PlayerVisibilityChangeEvent(ExploringEntity viewer, Tile tile, bool v)
        {
            this.Viewer = viewer;
            this.Tile = tile;
            this.TileVisible = v;
        }

        public ExploringEntity Viewer;
        public Tile Tile;
        public bool TileVisible;
    }
}

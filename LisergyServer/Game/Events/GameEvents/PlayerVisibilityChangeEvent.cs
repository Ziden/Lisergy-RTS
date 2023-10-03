using Game.Tile;

namespace Game.Events.GameEvents
{
    public class PlayerVisibilityChangeEvent : GameEvent
    {
        public PlayerVisibilityChangeEvent(BaseEntity viewer, TileEntity tile, bool v)
        {
            this.Viewer = viewer;
            this.Tile = tile;
            this.TileVisible = v;
        }

        public BaseEntity Viewer;
        public TileEntity Tile;
        public bool TileVisible;
    }
}

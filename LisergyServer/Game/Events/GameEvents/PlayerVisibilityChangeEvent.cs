using Game.Entity;


namespace Game.Events.GameEvents
{
    public class PlayerVisibilityChangeEvent : GameEvent
    {
        public PlayerVisibilityChangeEvent(WorldEntity viewer, Tile tile, bool v)
        {
            this.Viewer = viewer;
            this.Tile = tile;
            this.TileVisible = v;
        }

        public WorldEntity Viewer;
        public Tile Tile;
        public bool TileVisible;
    }
}

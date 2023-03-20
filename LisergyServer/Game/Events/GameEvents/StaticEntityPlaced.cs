

using Game.Tile;

namespace Game.Events.GameEvents
{
    public class BuildingPlacedEvent : GameEvent
    {
        public WorldEntity Entity;
        public TileEntity Tile;

        public BuildingPlacedEvent(WorldEntity entity, TileEntity tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

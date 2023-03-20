using Game.Tile;

namespace Game.Events.GameEvents
{
    public class BuildingRemovedEvent : GameEvent
    {
        public WorldEntity Entity;
        public TileEntity Tile;

        public BuildingRemovedEvent(WorldEntity entity, TileEntity tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}



using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
    public class BuildingPlacedEvent : GameEvent
    {
        public IEntity Entity;
        public TileEntity Tile;

        public BuildingPlacedEvent(IEntity entity, TileEntity tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

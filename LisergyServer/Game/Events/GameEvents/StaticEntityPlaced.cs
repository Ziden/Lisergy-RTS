

using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
    public class BuildingPlacedEvent : IGameEvent
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

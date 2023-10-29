using Game.ECS;
using Game.Events;
using Game.Tile;

namespace Game.Systems.Building
{
    /// <summary>
    /// Whenever a building is placed in the world
    /// </summary>
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

    /// <summary>
    /// Whenever a building is removed from the world
    /// </summary>
    public class BuildingRemovedEvent : IGameEvent
    {
        public IEntity Entity;
        public TileEntity Tile;

        public BuildingRemovedEvent(IEntity entity, TileEntity tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

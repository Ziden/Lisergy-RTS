using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
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

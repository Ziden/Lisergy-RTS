using Game.ECS;
using Game.Tile;

namespace Game.Events.GameEvents
{
    public class BuildingRemovedEvent : GameEvent
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

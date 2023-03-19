

namespace Game.Events.GameEvents
{
    public class BuildingPlacedEvent : GameEvent
    {
        public WorldEntity Entity;
        public Tile Tile;

        public BuildingPlacedEvent(WorldEntity entity, Tile tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

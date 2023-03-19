using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class BuildingPlacedEvent : GameEvent
    {
        public BuildingEntity Entity;
        public Tile Tile;

        public BuildingPlacedEvent(BuildingEntity entity, Tile tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

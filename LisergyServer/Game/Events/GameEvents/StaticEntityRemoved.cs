using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class BuildingRemovedEvent : GameEvent
    {
        public BuildingEntity Entity;
        public Tile Tile;

        public BuildingRemovedEvent(BuildingEntity entity, Tile tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

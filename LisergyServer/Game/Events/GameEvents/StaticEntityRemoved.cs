using Game.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class BuildingRemovedEvent : GameEvent
    {
        public WorldEntity Entity;
        public Tile Tile;

        public BuildingRemovedEvent(WorldEntity entity, Tile tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

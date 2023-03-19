using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class StaticEntityPlacedEvent : GameEvent
    {
        public StaticEntity Entity;
        public Tile Tile;

        public StaticEntityPlacedEvent(StaticEntity entity, Tile tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

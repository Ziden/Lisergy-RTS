using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class StaticEntityRemovedEvent : GameEvent
    {
        public StaticEntity Entity;
        public Tile Tile;

        public StaticEntityRemovedEvent(StaticEntity entity, Tile tile)
        {
            Entity = entity;
            Tile = tile;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class EntityMoveInEvent : GameEvent
    {
        public WorldEntity Entity;
        public Tile ToTile;
        public Tile FromTile;
    }

    public class EntityMoveOutEvent : GameEvent
    {
        public WorldEntity Entity;
        public Tile ToTile;
        public Tile FromTile;
    }
}

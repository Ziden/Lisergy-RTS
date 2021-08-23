using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class EntityMoveEvent : GameEvent
    {
        public WorldEntity Entity;
        public Tile NewTile;
        public Tile OldTile;
    }
}

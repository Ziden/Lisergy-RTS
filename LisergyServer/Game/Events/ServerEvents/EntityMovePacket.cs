using Game.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events
{
    [Serializable]
    public class EntityMovePacket : ServerEvent
    {
        public EntityMovePacket(MovableWorldEntity entity, Tile tile)
        {
            this.OwnerID = entity.OwnerID;
            this.ID = entity.Id;
            this.Delay = entity.GetMoveDelay();
            this.X = tile.X;
            this.Y = tile.Y;

        }

        public string OwnerID;
        public string ID;
        public TimeSpan Delay;
        public int X;
        public int Y;
    }
}

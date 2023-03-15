using Game.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events
{
    [Serializable]
    public class EntityMovePacket : ServerPacket
    {
        public EntityMovePacket(MovableWorldEntity entity, Tile tile)
        {
            this.OwnerID = entity.OwnerID;
            this.EntityID = entity.Id;
            this.Delay = entity.GetMoveDelay();
            this.X = tile.X;
            this.Y = tile.Y;

        }

        public GameId OwnerID;
        public GameId EntityID;
        public TimeSpan Delay;
        public ushort X;
        public ushort Y;
    }
}

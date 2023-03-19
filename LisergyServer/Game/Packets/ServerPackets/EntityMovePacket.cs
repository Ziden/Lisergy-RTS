using Game.DataTypes;
using Game.Entity;
using Game.Entity.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events
{
    [Serializable]
    public class EntityMovePacket : ServerPacket
    {
        public EntityMovePacket(WorldEntity entity, EntityMovementComponent component, Tile tile)
        {
            this.OwnerID = entity.OwnerID;
            this.EntityID = entity.Id;
            this.Delay = component.MoveDelay;
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

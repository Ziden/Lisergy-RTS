using Game.DataTypes;
using Game.Movement;
using Game.Tile;
using System;

namespace Game.Events
{
    [Serializable]
    public class EntityMovePacket : ServerPacket
    {
        public EntityMovePacket(WorldEntity entity, EntityMovementComponent component, TileEntity tile)
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

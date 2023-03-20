using Game.DataTypes;
using Game.Events;
using Game.Movement;
using Game.Tile;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class EntityMovePacket : ServerPacket
    {
        public EntityMovePacket(WorldEntity entity, EntityMovementComponent component, TileEntity tile)
        {
            OwnerID = entity.OwnerID;
            EntityID = entity.Id;
            Delay = component.MoveDelay;
            X = tile.X;
            Y = tile.Y;

        }

        public GameId OwnerID;
        public GameId EntityID;
        public TimeSpan Delay;
        public ushort X;
        public ushort Y;
    }
}

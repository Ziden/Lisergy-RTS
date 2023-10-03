using Game.ECS;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Systems.Player;
using System;

namespace Game.Tile
{
    public unsafe partial class TileEntity : IEntity, IDeltaTrackable, IDeltaUpdateable
    {
        private DeltaFlags _flags;
        public ref DeltaFlags DeltaFlags { get => ref _flags; }
        public void SetFlag(DeltaFlag flag) { }
        public ServerPacket GetUpdatePacket(PlayerEntity receiver) => new TileUpdatePacket(*_tileData);
        public void ProccessDeltas(PlayerEntity trigger)
        {
            if (DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
            {
                trigger.Send(GetUpdatePacket(trigger));
            }
        }
    }
}

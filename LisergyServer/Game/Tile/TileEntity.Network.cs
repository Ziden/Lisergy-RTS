using Game.ECS;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Player;
using System;

namespace Game.Tile
{
    public unsafe partial class TileEntity : IEntity, IDeltaTrackable, IDeltaUpdateable<TileUpdatePacket>
    {
        [field: NonSerialized]
        private DeltaFlags _flags;

        public ref DeltaFlags DeltaFlags { get => ref _flags; }

        public void SetFlag(DeltaFlag flag) { }

        public TileUpdatePacket GetUpdatePacket(PlayerEntity receiver) => new TileUpdatePacket(*_tileData);

        public void ProccessDeltas(PlayerEntity trigger)
        {
            if (DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
            {
                trigger.Send(GetUpdatePacket(trigger));
            }
        }
    }
}

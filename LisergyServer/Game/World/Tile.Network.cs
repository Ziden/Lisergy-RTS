using Game.ECS;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using System;

namespace Game
{
    public unsafe partial class Tile : IEntity, IDeltaTrackable, IDeltaUpdateable<TileUpdatePacket>
    {
        [field: NonSerialized]
        private DeltaFlags _flags;

        public ref DeltaFlags DeltaFlags { get => ref _flags; }

        public void SetFlag(DeltaFlag flag) { }

        public TileUpdatePacket UpdatePacket => new TileUpdatePacket(*_tileData);

        public void ProccessDeltas(PlayerEntity trigger)
        {
            if(DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
            {
                trigger.Send(UpdatePacket);
            }
        }
    }
}

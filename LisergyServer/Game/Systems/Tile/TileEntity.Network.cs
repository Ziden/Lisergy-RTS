using Game.ECS;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Systems.Player;
using System;
using System.Runtime.CompilerServices;

namespace Game.Tile
{
    public unsafe partial class TileEntity : IEntity, IEntityDeltaTrackable
    {
        private DeltaFlags _flags;
        public ref DeltaFlags DeltaFlags { get => ref _flags; }
        public BasePacket GetUpdatePacket(PlayerEntity receiver) => new TilePacket(*_tileData);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProccessDeltas(PlayerEntity trigger)
        {
            if (DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
            {
                Game.Network.SendToPlayer(GetUpdatePacket(trigger), trigger);
            }
        }
    }
}

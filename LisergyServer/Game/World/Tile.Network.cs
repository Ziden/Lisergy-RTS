using Game.ECS;
using Game.Events.ServerEvents;
using System;

namespace Game
{
    public unsafe partial class Tile : IEntity, IDeltaTrackable
    {
        [field: NonSerialized]
        public DeltaFlags DeltaFlags { get; }

        public void ProccessDeltas(PlayerEntity trigger)
        {
            if(DeltaFlags.HasFlag(DeltaFlag.REVEALED))
            {
                trigger.Send(new TileUpdatePacket(this));
            }
        }
    }
}

using Game.Entity;
using Game.Entity.Components;
using Game.Events;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.World.Components;
using System;
using System.Collections.Generic;

namespace Game
{
    public partial class WorldEntity : Ownable, IDeltaTrackable, IDeltaUpdateable<EntityUpdatePacket>
    {
        [field: NonSerialized]
        private DeltaFlags _flags;

        public ref DeltaFlags DeltaFlags { get => ref _flags; }

        public EntityUpdatePacket UpdatePacket => new EntityUpdatePacket(this);

        private static HashSet<PlayerEntity> viewersCache = new HashSet<PlayerEntity>();

        public void ProccessDeltas(PlayerEntity trigger)
        {
            if(DeltaFlags.HasFlag(DeltaFlag.EXISTENCE))
            {
                OnExistenceChanged();
            } else if (DeltaFlags.HasFlag(DeltaFlag.POSITION))
            {
                OnPositionChanged();
            }
            else if (DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
            {
                trigger.Send(UpdatePacket);
            }
        }

        private void OnExistenceChanged()
        {
            if (Tile == null) return; // was removed

            foreach(var playerViewing in Tile.PlayersViewing)
            {
                playerViewing.Send(new EntityUpdatePacket(this));
            }
        }

        private void OnPositionChanged()
        {
            var newTile = _tile;
            var previousTile = _previousTile;

            var movementComponent = this.Components.Get<EntityMovementComponent>();
            viewersCache.Clear();
            var allViewers = viewersCache;
            if (previousTile != newTile && previousTile != null)
            {
                allViewers.UnionWith(previousTile.Components.Get<TileVisibility>().PlayersViewing);
                if (newTile != null)
                    allViewers.UnionWith(newTile.Components.Get<TileVisibility>().PlayersViewing);

                var movePacket = new EntityMovePacket(this, movementComponent, newTile);
                foreach (var viewer in allViewers)
                    viewer.Send(movePacket);
            }

            var newPlayersViewing = new HashSet<PlayerEntity>(newTile.Components.Get<TileVisibility>().PlayersViewing);
            if (previousTile != null)
                newPlayersViewing.ExceptWith(previousTile.Components.Get<TileVisibility>().PlayersViewing);

            var packet = new EntityUpdatePacket(this);
            foreach (var viewer in newPlayersViewing)
                viewer.Send(packet);
        }
    }
}

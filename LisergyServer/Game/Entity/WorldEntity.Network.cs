using Game.Events;
using Game.Events.ServerEvents;
using Game.FogOfWar;
using Game.Movement;
using Game.Network.ServerPackets;
using Game.Network;
using Game.Player;
using System;
using System.Collections.Generic;

namespace Game
{
    public partial class WorldEntity : IDeltaTrackable, IDeltaUpdateable<EntityUpdatePacket>
    {
        [field: NonSerialized]
        private DeltaFlags _flags;

        public ref DeltaFlags DeltaFlags { get => ref _flags; }

        public static int x = 0;
        public EntityUpdatePacket GetUpdatePacket(PlayerEntity receiver)
        {
            var packet = new EntityUpdatePacket(this);
            packet.SyncedComponents = this.Components.GetSyncedComponents(receiver);
            return packet;
        }

        private static HashSet<PlayerEntity> viewersCache = new HashSet<PlayerEntity>();

        public void ProccessDeltas(PlayerEntity trigger)
        {
            if (DeltaFlags.HasFlag(DeltaFlag.EXISTENCE))
                OnExistenceChanged();
            else if (DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
                trigger.Send(GetUpdatePacket(trigger));
            if (DeltaFlags.HasFlag(DeltaFlag.POSITION))
                OnPositionChanged();
        }

        private void OnExistenceChanged()
        {
            if (Tile == null) return; // was removed

            foreach (var playerViewing in Tile.PlayersViewing)
            {
                playerViewing.Send(this.GetUpdatePacket(playerViewing));
            }
        }

        private void OnPositionChanged()
        {
            var newTile = _tile;
            var previousTile = _previousTile;

            var movementComponent = Components.Get<EntityMovementComponent>();
            if(movementComponent == null) return;
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

            foreach (var viewer in newPlayersViewing)
                viewer.Send(this.GetUpdatePacket(viewer));
        }
    }
}

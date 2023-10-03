using Game.Events;
using Game.Events.ServerEvents;
using Game.Network.ServerPackets;
using Game.Network;
using System;
using System.Collections.Generic;
using Game.Systems.Movement;
using Game.Systems.FogOfWar;
using Game.Systems.Player;

namespace Game
{
    public partial class BaseEntity : IDeltaTrackable
    {
        /// <summary>
        /// Cache to re-use the same hashset for all viewers lookups
        /// </summary>
        private static HashSet<PlayerEntity> ViewersCache = new HashSet<PlayerEntity>();

        [field: NonSerialized]
        private DeltaFlags _flags;

        public ref DeltaFlags DeltaFlags { get => ref _flags; }

        public static int x = 0;
        public ServerPacket GetUpdatePacket(PlayerEntity receiver)
        {
            var packet = new EntityUpdatePacket(this);
            packet.SyncedComponents = this.Components.GetSyncedComponents(receiver);
            return packet;
        }

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
            ViewersCache.Clear();
            var allViewers = ViewersCache;
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

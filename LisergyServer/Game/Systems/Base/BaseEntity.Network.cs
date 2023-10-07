using Game.Events.ServerEvents;
using Game.Network.ServerPackets;
using Game.Network;
using System;
using System.Collections.Generic;
using Game.Systems.Movement;
using Game.Systems.FogOfWar;
using Game.Systems.Player;
using Game.Systems.MapPosition;
using Game.ECS;
using System.Linq;

namespace Game
{
    public partial class BaseEntity
    {
        /// <summary>
        /// Cache to re-use the same hashset for all viewers lookups
        /// </summary>
        private static HashSet<PlayerEntity> ViewersCache = new HashSet<PlayerEntity>();

        [field: NonSerialized]
        private DeltaFlags _flags;

        public ref DeltaFlags DeltaFlags { get => ref _flags; }

        public ServerPacket GetUpdatePacket(PlayerEntity receiver)
        {
            var packet = new EntityUpdatePacket(this);
            packet.SyncedComponents = Components.GetSyncedComponents(receiver).ToArray();
            return packet;
        }

        public void ProccessDeltas(PlayerEntity trigger)
        {
            if (DeltaFlags.HasFlag(DeltaFlag.EXISTENCE))
                OnExistenceChanged();
            else if (DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
            {
                Game.Network.SendToPlayer(GetUpdatePacket(trigger), trigger);
            }
            if (DeltaFlags.HasFlag(DeltaFlag.COMPONENTS))
                SendUpdateToNewViewers();
        }

        private void OnExistenceChanged()
        {
            var c = Get<MapReferenceComponent>();
            if (c.Tile == null) return; 
            foreach (var playerViewing in c.Tile.PlayersViewing)
            {
                Game.Network.SendToPlayer(this.GetUpdatePacket(playerViewing), playerViewing);
            }
        }

        private void SendUpdateToNewViewers()
        {
            var c = Get<MapReferenceComponent>();
            var newTile = c.Tile;
            var previousTile = c.PreviousTile;

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
                Game.Network.SendToPlayer(movePacket, allViewers.ToArray());
            }

            var newPlayersViewing = new HashSet<PlayerEntity>(newTile.Components.Get<TileVisibility>().PlayersViewing);
            if (previousTile != null)
                newPlayersViewing.ExceptWith(previousTile.Components.Get<TileVisibility>().PlayersViewing);

            foreach (var viewer in newPlayersViewing)
                Game.Network.SendToPlayer(this.GetUpdatePacket(viewer), viewer);
        }
    }
}

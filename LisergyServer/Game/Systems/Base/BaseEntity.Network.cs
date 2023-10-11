using Game.Events.ServerEvents;
using Game.Network.ServerPackets;
using Game.Network;
using System.Collections.Generic;
using Game.Systems.Movement;
using Game.Systems.Player;
using Game.Systems.MapPosition;
using System.Linq;

namespace Game
{
    public partial class BaseEntity
    {
        /// <summary>
        /// Cache to re-use the same hashset for all viewers lookups
        /// </summary>
        private static HashSet<PlayerEntity> ViewersCache = new HashSet<PlayerEntity>();

        private DeltaFlags _flags;

        public ref DeltaFlags DeltaFlags { get => ref _flags; }

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
            var c = Components.GetReference<MapReferenceComponent>();
            if (c.Tile == null) return; 
            foreach (var playerViewing in c.Tile.PlayersViewing)
            {
                Game.Network.SendToPlayer(this.GetUpdatePacket(playerViewing), playerViewing);
            }
        }

        private void SendUpdateToNewViewers()
        {
            var c = Components.GetReference<MapReferenceComponent>();
            var newTile = c.Tile;
            var previousTile = c.PreviousTile;

            if (!Components.Has<CourseComponent>()) return;

            ViewersCache.Clear();
            var allViewers = ViewersCache;
            if (previousTile != newTile && previousTile != null)
            {
                allViewers.UnionWith(previousTile.PlayersViewing);
                if (newTile != null)
                    allViewers.UnionWith(newTile.PlayersViewing);

                var movePacket = new EntityMovePacket(this, Components.Get<CourseComponent>(), newTile);
                Game.Network.SendToPlayer(movePacket, allViewers.ToArray());
            }

            var newPlayersViewing = new HashSet<PlayerEntity>(newTile.PlayersViewing);
            if (previousTile != null)
                newPlayersViewing.ExceptWith(previousTile.PlayersViewing);

            foreach (var viewer in newPlayersViewing)
                Game.Network.SendToPlayer(this.GetUpdatePacket(viewer), viewer);
        }

        public BasePacket GetUpdatePacket(PlayerEntity receiver)
        {
            var packet = new EntityUpdatePacket(this);
            packet.SyncedComponents = Components.GetSyncedComponents(receiver).ToArray();
            return packet;
        }
    }
}

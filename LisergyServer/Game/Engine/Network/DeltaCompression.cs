using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.DeltaTracker;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Tile;
using System.Collections.Generic;

namespace Game.Engine.Network
{
    public class DeltaCompression
    {
        internal HashSet<IEntity> _modifiedEntities = new HashSet<IEntity>();
        internal HashSet<IEntity> _modifiedTiles = new HashSet<IEntity>();

        internal IGame _game;

        public bool Enabled = true;

        public DeltaCompression(IGame game)
        {
            _game = game;
        }

        public void AddModified(IEntity entity)
        {
            if (!Enabled) return;
            if (entity.EntityType == EntityType.Tile)
                _modifiedTiles.Add(entity);
            else
                _modifiedEntities.Add(entity);
        }

        public void ClearDeltas()
        {
            if (!Enabled) return;
            foreach (var tracked in _modifiedEntities)
            {
                tracked.Logic.DeltaCompression.Clear();
            }
            foreach (var tracked in _modifiedTiles)
            {
                tracked.Logic.DeltaCompression.Clear();
            }
            _modifiedEntities.Clear();
            _modifiedTiles.Clear();
        }

        public void SendAllModifiedEntities(GameId trigger)
        {
            if (!Enabled) return;

            // We always send all tiles first
            foreach (var tracked in _modifiedTiles)
            {
                ProccessDeltas(tracked, trigger);
                tracked.Logic.DeltaCompression.Clear();
            }

            foreach (var tracked in _modifiedEntities)
            {
                ProccessDeltas(tracked, trigger);
                tracked.Logic.DeltaCompression.Clear();
            }

            _modifiedTiles.Clear();
            _modifiedEntities.Clear();
        }

        /// <summary>
        /// Cache to re-use the same hashset for all viewers lookups
        /// </summary>
        private static HashSet<GameId> _stoppedSeeingEntity = new HashSet<GameId>();

        public void SendEntityPacket(GameId entityId, GameId playerId, bool onlyDelta = true)
        {
            var entity = _game.Entities[entityId];
            var packet = entity.Logic.DeltaCompression.GetUpdatePacket(playerId, onlyDelta);
            _game.Network.SendToPlayer(packet, playerId);
        }

        /// <summary>
        /// We send component updates to all viwers
        /// But if the map position has updated then we also need to send the update
        /// for the old viwers so they can see the entity moving our of their view
        /// </summary>
        private void SendUpdateToNewViewers(IEntity entity)
        {
            //_game.Log.Debug($"Entity {entity} Had DeltaFlag 'COMPONENTS' - Sending Packets");
            var hasPlacement = entity.Components.TryGet<MapPlacementComponent>(out var placement);
            var hasPreviousTile = entity.Components.TryGet<PreviousMapPlacementComponent>(out var previousPlacement);

            placement = placement ?? new MapPlacementComponent();

            if (entity.Components.TryGet<TileDataComponent>(out var tileData))
            {
                var tile = _game.World.GetTile(tileData.Position);
                foreach (var playerId in tile.Logic.Vision.GetPlayersViewing())
                {
                    SendEntityPacket(entity.EntityId, playerId);
                }
                return;
            }
            if (!entity.Components.Has<MovementComponent>()) return;

            var newTile = _game.World.GetTile(placement.Position);

            if (placement.Position != previousPlacement?.Position)
            {
                if (hasPreviousTile)
                {
                    entity.Components.Remove<PreviousMapPlacementComponent>(); // TODO WTF ?
                }

                _stoppedSeeingEntity.Clear();

                if (hasPreviousTile)
                    _stoppedSeeingEntity.UnionWith(
                    _game.World.GetTile(previousPlacement.Position).Logic.Vision.GetPlayersViewing());

                if (newTile != null)
                    _stoppedSeeingEntity.ExceptWith(newTile.Logic.Vision.GetPlayersViewing());

                foreach (var stoppedSeeingEntity in _stoppedSeeingEntity)
                    SendEntityPacket(entity.EntityId, stoppedSeeingEntity);
            }

            foreach (var seeingEntity in newTile.Logic.Vision.GetPlayersViewing())
                SendEntityPacket(entity.EntityId, seeingEntity);
        }

        public void ProccessDeltas(IEntity e, GameId trigger)
        {
            e.Components.TryGet<DeltaFlagsComponent>(out var deltaFlags);
            if (deltaFlags.HasFlag(DeltaFlag.CREATED)) OnExistenceChanged(e);
            else if (deltaFlags.HasFlag(DeltaFlag.COMPONENTS)) SendUpdateToNewViewers(e);
            else if (deltaFlags.HasFlag(DeltaFlag.SELF_REVEALED)) OnRevealed(e, trigger);
        }

        private void OnExistenceChanged(IEntity e)
        {
            var placementTile = e.Logic.Map.GetTile();
            if (placementTile == null) return;

            //_game.Log.Debug($"Entity {e} Had DeltaFlag 'EXISTENCE' - Sending Packets");
            foreach (var playerViewing in placementTile.Logic.Vision.GetPlayersViewing())
                SendEntityPacket(e.EntityId, playerViewing, false);
        }

        private void OnRevealed(IEntity e, GameId trigger)
        {
            // _game.Log.Debug($"Entity {e} Had DeltaFlag 'REVEALED' - Sending Packets");
            SendEntityPacket(e.EntityId, trigger, false);
        }
    }
}

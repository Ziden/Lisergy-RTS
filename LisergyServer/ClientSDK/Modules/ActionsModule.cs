using ClientSDK.Data;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Systems.Course;
using Game.Systems.Harvesting;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Tile;
using System.Linq;

namespace ClientSDK.Services
{
    /// <summary>
    /// Module for player input actions. This module encapsulates all potential actions the player can do on the game using network
    /// </summary>
    public interface IActionModule : IClientModule
    {
        /// <summary>
        /// Tries to move the given entity to the target destination.
        /// Will return true or false if the entity is able to move there or not.
        /// </summary>
        bool MoveEntity(IEntity e, TileModel toTile, CourseIntent intent);

        /// <summary>
        /// Stops the party for any actions.
        /// </summary>
        bool StopEntity(IEntity party);
    }

    public class ActionsModule : IActionModule
    {
        private GameClient _client;

        public ActionsModule(GameClient client)
        {
            _client = client;
        }

        public void Register()
        {

        }

        public bool MoveEntity(IEntity entity, TileModel destinationTile, CourseIntent intent)
        {
            if (entity == null)
            {
                _client.SDKLog.Error($"Error invalid entity");
                return false;
            }
            var entityId = entity.EntityId;
            if (entity.OwnerID != _client.Modules.Player.PlayerId)
            {
                _client.SDKLog.Error($"Cannot Move Entity {entityId} is not own entity");
                return false;
            }
            if (!entity.Components.TryGet<MapPlacementComponent>(out var placement))
            {
                _client.SDKLog.Error($"Cannot Move Entity {entityId} it is not placed in the map");
                return false;
            }
            var w = _client.Game.World;
            var sourceTile = w.GetTile(placement.Position.X, placement.Position.Y);
            if (!destinationTile.Logic.Vision.GetPlayersViewing().Any(p => p == entity.OwnerID))
            {
                _client.SDKLog.Error($"Cannot Move Entity {entityId} because target tile is not visible");
                return false;
            }
            var path = w.FindPath(sourceTile, destinationTile);
            if (path == null || path.Count() == 0)
            {
                _client.SDKLog.Error($"Cannot Move Entity {entityId} it is not placed in the map");
                return false;
            }
            foreach (var pathNode in path)
            {
                var tile = w.GetTile(pathNode.X, pathNode.Y);
                if (tile == null)
                {
                    _client.SDKLog.Error($"Trying to walk path in {pathNode.X} {pathNode.Y} but tile was not yet received");
                    return false;
                }
                var tileView = _client.Modules.Views.GetEntityView(tile.TileEntity);

                if (tileView == null || tileView.State == EntityViewState.NOT_RENDERED)
                {
                    _client.SDKLog.Error($"Cannot Move Entity {entityId} by a path that is not known by the client");
                    return false;
                }
            }
            _client.SDKLog.Debug($"Sending request to move party {entity} {path.Count()} tiles");
            _client.ClientEvents.Call(new EntityMovementRequestStarted()
            {
                Destination = destinationTile,
                Path = path,
                Intent = intent,
                Party = entity,

            });
            _client.Network.SendToServer(new MoveEntityCommand()
            {
                Entity = entity.EntityId,
                Intent = intent,
                Path = path.ToList()
            });
            return true;
        }

        public bool StopEntity(IEntity party)
        {
            _client.Network.SendToServer(new StopHarvestingCommand()
            {
                EntityId = party.EntityId,
            });
            return true;
        }
    }
}

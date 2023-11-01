using ClientSDK.Data;
using ClientSDK.Modules;
using ClientSDK.SDKEvents;
using Game;
using Game.DataTypes;

using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Party;
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
        bool MoveParty(PartyEntity party, TileEntity toTile, CourseIntent intent);
    }

    public class ActionsModule : IActionModule
    {
        private IGameClient _client;

        public ActionsModule(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
          
        }

        public bool MoveParty(PartyEntity entity, TileEntity destinationTile, CourseIntent intent)
        {
            if (entity == null)
            {
                _client.Log.Error($"Error invalid entity");
                return false;
            }
            var entityId = entity.EntityId;
            if (entity.OwnerID != _client.Modules.Player.PlayerId)
            {
                _client.Log.Error($"Cannot Move Entity {entityId} is not own entity");
                return false;
            }
            if(!entity.Components.TryGet<MapPlacementComponent>(out var placement))
            {
                _client.Log.Error($"Cannot Move Entity {entityId} it is not placed in the map");
                return false;
            }
            var map = _client.Game.World.Map;
            var sourceTile = map.GetTile(placement.Position.X, placement.Position.Y);
            if (!destinationTile.PlayersViewing.Any(p => p.EntityId == entity.OwnerID))
            {
                _client.Log.Error($"Cannot Move Entity {entityId} because target tile is not visible");
                return false;
            }
            var path = map.FindPath(sourceTile, destinationTile);
            if(path == null || path.Count() == 0)
            {
                _client.Log.Error($"Cannot Move Entity {entityId} it is not placed in the map");
                return false;
            }
            foreach(var pathNode in path)
            {
                var tile = map.GetTile(pathNode.X, pathNode.Y);
                var tileView = _client.Modules.Views.GetEntityView(tile);

                if(tileView == null || tileView.State == EntityViewState.NOT_RENDERED)
                {
                    _client.Log.Error($"Cannot Move Entity {entityId} by a path that is not known by the client");
                    return false;
                }
            }
            _client.Log.Debug($"Sending request to move party {entity} {path.Count()} tiles");
            _client.ClientEvents.Call(new EntityMovementRequestStarted()
            {
                Destination = destinationTile,
                Path = path,
                Intent = intent,
                Party = entity,
                
            });
            _client.Network.SendToServer(new MoveRequestPacket()
            {
                PartyIndex = entity.PartyIndex,
                Intent = intent,
                Path = path.ToList()
            });
            return true;
        }
    }
}

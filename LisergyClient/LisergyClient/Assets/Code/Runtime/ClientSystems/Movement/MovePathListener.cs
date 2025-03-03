using Assets.Code.Assets.Code.Runtime.Movement;
using Assets.Code.Views;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.UI
{
    /// <summary>
    /// Responsible for rendering the visual representation of game paths.
    /// It listens for events for when to render the movement path on the map and events for when to hide it
    /// </summary>
    public class MovePathListener : IEventListener
    {
        private IGameClient _gameClient;
        private IDictionary<GameId, MovePathView> _entityPaths = new DefaultValueDictionary<GameId, MovePathView>();

        public MovePathListener(IGameClient _client)
        {
            _gameClient = _client;
            _client.ClientEvents.On<EntityMovementRequestStarted>(this, OnMoveRequestStarted);
            //UIEvents.OnCourseCancelled += OnCourseChanged;
        }

        private void OnCourseChanged(IEntity p)
        {
            if(_entityPaths.TryGetValue(p.EntityId, out var path))
            {
                path.Clear();
                _entityPaths.Remove(p.EntityId);
            }
        }

        private void OnMoveRequestStarted(EntityMovementRequestStarted ev)
        {
            var party = ev.Party;
            var path = ev.Path.Select(p => _gameClient.Game.World.GetTile(p.X, p.Y)).ToList();
            var server = _gameClient.Modules;
            if (_entityPaths.TryGetValue(party.EntityId, out var previousPath))
            {
                previousPath.Clear();
                _entityPaths.Remove(party.EntityId);
            }

            var clientPath = new MovePathView();
            _entityPaths[party.EntityId] = clientPath;
            for (var x = 0; x < path.Count; x++)
            {
                var nodeTile = path[x];
                var view = nodeTile.TileEntity.GetView();
                var hasNext = x < path.Count - 1;
                var hasPrevious = x > 0;
                if (hasNext)
                {
                    var next = path[x + 1];
                    var direction = nodeTile.GetDirection(next);
                    _ = clientPath.AddPath(server.Views.GetEntityView(next.TileEntity) as TileView, view.GameObject.transform.position, direction);
                }
                if (hasPrevious)
                {
                    var previous = path[x - 1];
                    var direction = nodeTile.GetDirection(previous);
                    _ = clientPath.AddPath(server.Views.GetEntityView(nodeTile.TileEntity) as TileView, view.GameObject.transform.position, direction);
                }
            }
        }

        public void OnFinishedMove(IEntity entity, TileModel newTile)
        {
            if (_entityPaths.ContainsKey(entity.EntityId))
            {
                var partyPath = _entityPaths[entity.EntityId];
                var pathsOnTile = partyPath.Pop(newTile.TileEntity);
                if (pathsOnTile != null)
                    foreach (var path in pathsOnTile)
                        partyPath.RemovePathObject(path);
                if (partyPath.Empty())
                    _entityPaths.Remove(entity.EntityId);
            }
        }
    }
}

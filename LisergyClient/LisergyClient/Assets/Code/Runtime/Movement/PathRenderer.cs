using Assets.Code.Assets.Code.Runtime.Movement;
using Assets.Code.Views;
using ClientSDK;

using Game.DataTypes;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using System.Collections.Generic;


namespace Assets.Code.UI
{
    public class PathRenderer
    {
        private IDictionary<GameId, MovementPath> _entityPaths = new DefaultValueDictionary<GameId, MovementPath>();

        public PathRenderer()
        {
            ClientEvents.OnPartyFinishedMove += OnFinishedMove;
            ClientEvents.OnStartMovementRequest += StartReqMove;
            ClientEvents.OnCourseCancelled += OnCourseChanged;
        }

        private void OnCourseChanged(PartyEntity p)
        {
            if(_entityPaths.TryGetValue(p.EntityId, out var path))
            {
                path.Clear();
                _entityPaths.Remove(p.EntityId);
            }
        }

        public void StartReqMove(PartyEntity party, List<TileEntity> path)
        {
            var server = ServiceContainer.Resolve<IServerModules>();
            if (_entityPaths.TryGetValue(party.EntityId, out var previousPath))
            {
                previousPath.Clear();
                _entityPaths.Remove(party.EntityId);
            }

            var clientPath = new MovementPath();
            _entityPaths[party.EntityId] = clientPath;
            for (var x = 0; x < path.Count; x++)
            {
                var nodeTile = path[x];
                var view = server.Views.GetView<TileView>(nodeTile);
                var hasNext = x < path.Count - 1;
                var hasPrevious = x > 0;
                if (hasNext)
                {
                    var next = path[x + 1];
                    var direction = nodeTile.GetDirection(next);
                    clientPath.AddPath(server.Views.GetView<TileView>(next), view.GameObject.transform.position, direction);
                }
                if (hasPrevious)
                {
                    var previous = path[x - 1];
                    var direction = nodeTile.GetDirection(previous);
                    clientPath.AddPath(server.Views.GetView<TileView>(nodeTile), view.GameObject.transform.position, direction);
                }
            }
        }

        public void OnFinishedMove(PartyEntity party, TileEntity oldTile, TileEntity newTile)
        {
            if (_entityPaths.ContainsKey(party.EntityId))
            {
                var partyPath = _entityPaths[party.EntityId];
                var pathsOnTile = partyPath.Pop(newTile);
                if (pathsOnTile != null)
                    foreach (var path in pathsOnTile)
                        partyPath.RemovePathObject(path);
                if (partyPath.Empty())
                    _entityPaths.Remove(party.EntityId);
            }
        }
    }
}

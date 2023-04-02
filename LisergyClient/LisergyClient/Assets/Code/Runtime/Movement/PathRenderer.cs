using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.Movement;
using Assets.Code.Views;
using Game;
using Game.DataTypes;
using Game.Party;
using Game.Tile;
using Game.World;
using GameAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            if(_entityPaths.TryGetValue(p.Id, out var path))
            {
                path.Clear();
                _entityPaths.Remove(p.Id);
            }
        }

        public void StartReqMove(PartyEntity party, List<TileEntity> path)
        {
            if (_entityPaths.TryGetValue(party.Id, out var previousPath))
            {
                previousPath.Clear();
                _entityPaths.Remove(party.Id);
            }

            var clientPath = new MovementPath();
            _entityPaths[party.Id] = clientPath;
            for (var x = 0; x < path.Count; x++)
            {
                var nodeTile = path[x];
                var view = GameView.GetView<TileView>(nodeTile);
                var hasNext = x < path.Count - 1;
                var hasPrevious = x > 0;
                if (hasNext)
                {
                    var next = path[x + 1];
                    var direction = nodeTile.GetDirection(next);
                    clientPath.AddPath(GameView.GetView<TileView>(next), view.GameObject.transform.position, direction);
                }
                if (hasPrevious)
                {
                    var previous = path[x - 1];
                    var direction = nodeTile.GetDirection(previous);
                    clientPath.AddPath(GameView.GetView<TileView>(nodeTile), view.GameObject.transform.position, direction);
                }
            }
        }

        public void OnFinishedMove(PartyEntity party, TileEntity oldTile, TileEntity newTile)
        {
            if (_entityPaths.ContainsKey(party.Id))
            {
                var partyPath = _entityPaths[party.Id];
                var pathsOnTile = partyPath.Pop(newTile);
                if (pathsOnTile != null)
                    foreach (var path in pathsOnTile)
                        partyPath.RemovePathObject(path);
                if (partyPath.Empty())
                    _entityPaths.Remove(party.Id);
            }
        }
    }
}

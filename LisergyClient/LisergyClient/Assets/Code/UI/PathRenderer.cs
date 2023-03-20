using Assets.Code.Views;
using Game;
using Game.Party;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.UI
{
    public class ClientPath {
        internal Dictionary<TileEntity, List<GameObject>> _pathLines = new Dictionary<TileEntity, List<GameObject>>();

        internal void Add(TileEntity tile, params GameObject [] pathLines)
        {
            if (!_pathLines.ContainsKey(tile))
                _pathLines[tile] = new List<GameObject>();
            _pathLines[tile].AddRange(pathLines);
        }

        internal List<GameObject> Pop(TileEntity tile)
        {
            if(_pathLines.ContainsKey(tile))
            {
                var p = _pathLines[tile];
                _pathLines.Remove(tile);
                return p;
            }
            return null;
        }

        internal bool Empty() => _pathLines.Count == 0;
    }

    public class PathRenderer
    {
        private Dictionary<PartyEntity, ClientPath> _partyPaths = new Dictionary<PartyEntity, ClientPath>();
        private List<GameObject> _pathlinesPool = new List<GameObject>();

        public PathRenderer()
        {
            ClientEvents.OnPartyFinishedMove += OnFinishedMove;
            ClientEvents.OnStartMovementRequest += StartReqMove;
        }

        public void StartReqMove(PartyEntity party, List<TileEntity> path)
        {
            this.RenderPath(party, path);
        }

        public void OnFinishedMove(PartyEntity party, TileEntity oldTile, TileEntity newTile)
        {
            if(_partyPaths.ContainsKey(party))
            {
                var partyPath = _partyPaths[party];
                var pathsOnTile = partyPath.Pop(newTile);
                var pathsOnOldTile = partyPath.Pop(oldTile);
                if(pathsOnTile != null)
                    foreach(var path in pathsOnTile)
                        path.SetActive(false);
                if (pathsOnOldTile != null)
                    foreach (var path in pathsOnOldTile)
                        path.SetActive(false);

                if (partyPath.Empty())
                    _partyPaths.Remove(party);
            }
        }


        private GameObject GetOrCreatePathLine(TileEntity tile, ClientPath clientPath)
        {
            var pooled = _pathlinesPool.FirstOrDefault(path => !path.activeInHierarchy);
            if (pooled == null)
            {
                Log.Debug("Created new path line");
                pooled = MainBehaviour.Instantiate(Resources.Load("prefabs/HalfPath")) as GameObject;
                _pathlinesPool.Add(pooled);
            } else
            {
                pooled.SetActive(true);
                pooled.transform.rotation = Quaternion.identity;
            }
            clientPath.Add(tile, pooled);
            return pooled;
        }

        public ClientPath RenderPath(PartyEntity party, List<TileEntity> tilePath)
        {
            //tilePath.RemoveAt(0); // remove where the party is
            var clientPath = new ClientPath();
            _partyPaths[party] = clientPath;
            _pathlinesPool.ForEach(path => path.SetActive(false));
            for (var x = 0; x < tilePath.Count; x++)
            {
                var nodeTile = tilePath[x];
                var tileView = GameView.GetView<TileView>(nodeTile);
                var tilePos = tileView.GameObject.transform.position;
                var hasNext = x < tilePath.Count - 1;
                var hasPrevious = x > 0;
                if (hasNext)
                {
                    var next = tilePath[x + 1];
                    var direction = nodeTile.GetDirection(next);
                    if(direction == Direction.SOUTH)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z - 0.25f);
                    }
                    else if (direction == Direction.NORTH)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z + 0.25f);
                    }
                    else if (direction == Direction.EAST)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x + 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                    else if (direction == Direction.WEST)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x - 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                }
                if (hasPrevious)
                {
                    var previous = tilePath[x - 1];
                    var direction = nodeTile.GetDirection(previous);
                    if (direction == Direction.NORTH)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z + 0.25f);
                    } else if (direction == Direction.SOUTH)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z - 0.25f);
                    }
                    else if (direction == Direction.EAST)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x + 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                    else if (direction == Direction.WEST)
                    {
                        var line = GetOrCreatePathLine(nodeTile, clientPath);
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x - 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                }
            }
            return clientPath;
        }
    }
}

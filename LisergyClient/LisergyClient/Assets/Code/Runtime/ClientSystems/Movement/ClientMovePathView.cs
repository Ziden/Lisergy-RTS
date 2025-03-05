using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.Tools;
using Assets.Code.Views;
using Cysharp.Threading.Tasks;
using Game.Engine.ECLS;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using GameAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Assets.Code.Runtime.Movement
{
    /// <summary>
    /// Visual representation of a movement path
    /// </summary>
    public class ClientMovePathView
    {
        private static ActivationObjectPool _pool = new ActivationObjectPool();

        internal Dictionary<IEntity, List<GameObject>> _pathsPerTile = new Dictionary<IEntity, List<GameObject>>();
        private IAssetService _assets;

        public ClientMovePathView()
        {
            _assets = UnityServicesContainer.Resolve<IAssetService>();
        }

        internal void Add(IEntity tile, params GameObject[] pathLines)
        {
            if (!_pathsPerTile.ContainsKey(tile))
                _pathsPerTile[tile] = new List<GameObject>();
            _pathsPerTile[tile].AddRange(pathLines);
        }

        internal List<GameObject> Pop(IEntity tile)
        {
            if (_pathsPerTile.ContainsKey(tile))
            {
                var p = _pathsPerTile[tile];
                _pathsPerTile.Remove(tile);
                return p;
            }
            return null;
        }

        public async UniTask<GameObject> AddPathObject(Vector3 position, Quaternion rotation = default)
        {
            var pooled = _pool.Obtain();
            if (pooled != null)
            {
                pooled.transform.SetPositionAndRotation(position, rotation);
                return pooled;
            }
            else
            {
                var o = await _assets.CreateVfx(VfxPrefab.HalfPath, position, rotation);
                _pool.AddNew(o);
                return o;
            }
        }

        public void Clear()
        {
            foreach (var kp in _pathsPerTile)
                foreach (var v in kp.Value)
                    AddBackToPool(v);
            _pathsPerTile.Clear();
        }

        public void AddBackToPool(GameObject path) => _pool.Release(path);

        public async UniTask AddPath(TileView addOnTile, Vector3 tilePos, Direction direction)
        {
            if (direction == Direction.SOUTH)
            {
                Add(addOnTile.Entity, await AddPathObject(new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z - 0.25f)));
            }
            else if (direction == Direction.NORTH)
            {
                Add(addOnTile.Entity, await AddPathObject(new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z + 0.25f)));
            }
            else if (direction == Direction.EAST)
            {
                var o = await AddPathObject(new Vector3(tilePos.x + 0.25f, tilePos.y + 0.1f, tilePos.z));
                o.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                Add(addOnTile.Entity, o);
            }
            else if (direction == Direction.WEST)
            {
                var o = await AddPathObject(new Vector3(tilePos.x - 0.25f, tilePos.y + 0.1f, tilePos.z));
                o.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                Add(addOnTile.Entity, o);
            }
        }

        internal bool Empty() => _pathsPerTile.Count == 0;
    }
}
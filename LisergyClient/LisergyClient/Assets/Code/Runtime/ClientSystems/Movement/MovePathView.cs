using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.Tools;
using Assets.Code.Views;
using Cysharp.Threading.Tasks;
using Game.Systems.Tile;
using Game.Tile;
using GameAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Assets.Code.Runtime.Movement
{
    /// <summary>
    /// Visual representation of a movement path
    /// </summary>
    public class MovePathView
    {
        private static ActivationObjectPool _pool = new ActivationObjectPool();

        internal Dictionary<TileEntity, List<GameObject>> _pathLines = new Dictionary<TileEntity, List<GameObject>>();
        private IAssetService _assets;

        public MovePathView()
        {
            _assets = UnityServicesContainer.Resolve<IAssetService>();
        }

        internal void Add(TileEntity tile, params GameObject[] pathLines)
        {
            if (!_pathLines.ContainsKey(tile))
                _pathLines[tile] = new List<GameObject>();
            _pathLines[tile].AddRange(pathLines);
        }

        internal List<GameObject> Pop(TileEntity tile)
        {
            if (_pathLines.ContainsKey(tile))
            {
                var p = _pathLines[tile];
                _pathLines.Remove(tile);
                return p;
            }
            return null;
        }

        public async UniTask AddPathObject(Vector3 position, Action<GameObject> onCreated = null)
        {
            var pooled = _pool.Obtain();
            if (pooled != null)
            {
                pooled.transform.SetPositionAndRotation(position, Quaternion.identity);
                onCreated(pooled);
            }
            else
            {
                var o = await _assets.CreateVfx(VfxPrefab.HalfPath, position, Quaternion.identity);
                _pool.AddNew(o);
                onCreated(o);
            }
        }

        public void Clear()
        {
            foreach (var kp in _pathLines)
                foreach (var v in kp.Value)
                    RemovePathObject(v);
            _pathLines.Clear();
        }

        public void RemovePathObject(GameObject path) => _pool.Release(path);

        public async UniTaskVoid AddPath(TileView addOnTile, Vector3 tilePos, Direction direction)
        {
            if (direction == Direction.SOUTH)
            {
                await AddPathObject(new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z - 0.25f), o =>
                {
                    Add(addOnTile.Entity, o);
                });
            }
            else if (direction == Direction.NORTH)
            {
                await AddPathObject(new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z + 0.25f), o =>
                {
                    Add(addOnTile.Entity, o);
                });
            }
            else if (direction == Direction.EAST)
            {
                await AddPathObject(new Vector3(tilePos.x + 0.25f, tilePos.y + 0.1f, tilePos.z), o =>
                {
                    Add(addOnTile.Entity, o);
                    o.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                });
            }
            else if (direction == Direction.WEST)
            {
                await AddPathObject(new Vector3(tilePos.x - 0.25f, tilePos.y + 0.1f, tilePos.z), o =>
                {
                    Add(addOnTile.Entity, o);
                    o.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                });
            }
        }

        internal bool Empty() => _pathLines.Count == 0;
    }
}

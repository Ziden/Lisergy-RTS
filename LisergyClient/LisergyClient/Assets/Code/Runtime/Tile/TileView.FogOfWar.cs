using Assets.Code.Views;
using Game.Tile;
using Game.World;
using GameAssets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<TileEntity>
    {
        private static GameObject _fogContainer;
        private bool? _withFog = null;
        private List<GameObject> _fogs = new List<GameObject>(5);

        public bool HasFog => _fogs.Count > 0;

        private void InitializeFog()
        {
            SetFogInTileView(true, false);
            if (Entity.GetNeighbor(Direction.SOUTH) == null) AddMapBorderFog(Entity.X, Entity.Y - 1);
            if (Entity.GetNeighbor(Direction.NORTH) == null) AddMapBorderFog(Entity.X, Entity.Y + 1);
            if (Entity.GetNeighbor(Direction.EAST) == null) AddMapBorderFog(Entity.X + 1, Entity.Y);
            if (Entity.GetNeighbor(Direction.WEST) == null) AddMapBorderFog(Entity.X - 1, Entity.Y);
        }

        private void AddMapBorderFog(int x, int y)
        {
            if (_fogContainer == null) _fogContainer = new GameObject("FogContainer");
            _assets.CreateTile(TilePrefab.FogBlack, new Vector3(x, 0.1f, y), Quaternion.Euler(0, 0, 0), f => f.transform.parent = _fogContainer.transform);
        }

        private void AddFogPrefabToTile(TilePrefab fogFab, int offsetX = 0, int offsetY = 0, int rotation = 0)
        {
            _assets.CreateTile(fogFab, new Vector3(Entity.X + offsetX, 0.1f, Entity.Y + offsetY), Quaternion.Euler(0, rotation, 0),
            o =>
            {
                if (_withFog.HasValue && !_withFog.Value)
                {
                    Object.Destroy(o);
                    return;
                }
                o.transform.parent = _fogContainer.transform;
                _fogs.Add(o);
            });
        }

        public void SetFogInTileView(bool fog, bool halfAlpha)
        {
            _withFog = fog;
            if (fog)
            {
                if (_fogContainer == null) _fogContainer = new GameObject("FogContainer");
                var prefab = halfAlpha ? TilePrefab.Fog50 : TilePrefab.FogBlack;
                AddFogPrefabToTile(prefab);
            }
            else if (_fogs.Count > 0)
            {
                foreach (var f in _fogs)
                {
                    Object.Destroy(f);
                }
                _fogs.Clear();
            }
        }
    }
}

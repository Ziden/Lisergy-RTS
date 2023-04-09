
using Assets.Code.Assets.Code.Assets;
using Game;
using Game.Party;
using Game.Tile;
using GameAssets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class TileUI
    {
        private TileEntity _selectedTile;

        private GameObject _tileCursor;

        public TileUI()
        {
            ServiceContainer.InputManager().OnClickTile += ClickTile;
            ClientEvents.OnCameraMove += OnCameraMove;
            ClientEvents.OnStartMovementRequest += OnStartMovement;
            var assets = ServiceContainer.Resolve<IAssetService>();
            assets.CreateMapObject(MapObjectPrefab.Cursor, Vector3.zero, Quaternion.Euler(0, 0, 0), o =>
            {
                o.SetActive(false);
                _tileCursor = o;
            });
        }

        public void OnStartMovement(PartyEntity party, List<TileEntity> path)
        {
            if (IsActive(_tileCursor))
                Inactivate(_tileCursor);
            _selectedTile = null;
        }

        private void OnCameraMove(Vector3 old, Vector3 newPos)
        {
            if (IsActive(_tileCursor))
                Inactivate(_tileCursor);
            _selectedTile = null;
        }

        public TileEntity SelectedTile { get => _selectedTile; }

        private void ClickTile(TileEntity tile)
        {
            if (tile == null) return;
            Log.Debug($"TileUI selecting tile {tile}");
            foreach(var e in tile.EntitiesViewing)
            {
                Log.Debug(""+e);
            }
           
            if (tile != null)
            {
                if (!IsActive(_tileCursor))
                    Activate(_tileCursor);
                MoveToTile(_tileCursor, tile);
                _selectedTile = tile;
            }
            ClientEvents.ClickTile(tile);
        }

        private void MoveToTile(GameObject cursor, TileEntity tile)
        {
            cursor.transform.position = new Vector3(tile.X, 0, tile.Y);
        }

        private bool IsActive(GameObject cursor)
        {
            return cursor.activeInHierarchy;
        }

        private void Activate(GameObject cursor)
        {
            cursor.SetActive(true);
        }

        private void Inactivate(GameObject cursor)
        {
            cursor.SetActive(false);
        }
    }
}

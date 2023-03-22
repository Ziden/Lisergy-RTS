
using Game;
using Game.Party;
using Game.Tile;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class TileUI
    {
        private TileEntity _selectedTile;

        private GameObject _tileCursor;
        private GameObject _partyCursor;

        public TileUI()
        {
            Global.InputManager().OnClickTile += ClickTile;
            ClientEvents.OnCameraMove += CameraMove;
            ClientEvents.OnSelectParty += SelectParty;
            ClientEvents.OnStartMovementRequest += StartMoveReq;

            _tileCursor = CreateCursor();
            _partyCursor = CreateCursor();
        }

        public void StartMoveReq(PartyEntity party, List<TileEntity> path)
        {
            if (IsActive(_tileCursor))
                Inactivate(_tileCursor);
            _selectedTile = null;
        }

        private void SelectParty(PartyEntity party)
        {
            Activate(_partyCursor);
            MoveToTile(_partyCursor, party.Tile);
            var view = GameView.GetView(party);
            _partyCursor.transform.SetParent(view.GameObject.transform);
        }

        private void CameraMove(Vector3 old, Vector3 newPos)
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

        private GameObject CreateCursor()
        {
            var prefab = Resources.Load("prefabs/tiles/Cursor");
            var cursor = MainBehaviour.Instantiate(prefab) as GameObject;
            cursor.SetActive(false);
            return cursor;
        }
    }
}


using Assets.Code.World;
using Game;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class TileUI
    {
        private ClientTile _selectedTile;

        private GameObject _tileCursor;
        private GameObject _partyCursor;

        public TileUI()
        {
            ClientEvents.OnClickTile += ClickTile;
            ClientEvents.OnCameraMove += CameraMove;
            ClientEvents.OnSelectParty += SelectParty;
            ClientEvents.OnStartMovementRequest += StartMoveReq;

            _tileCursor = CreateCursor();
            _partyCursor = CreateCursor();
        }

        public void StartMoveReq(ClientParty party, List<ClientTile> path)
        {
            if (IsActive(_tileCursor))
                Inactivate(_tileCursor);
            _selectedTile = null;
        }

        private void SelectParty(ClientParty party)
        {
            Activate(_partyCursor);
            MoveToTile(_partyCursor, party.Tile);
            _partyCursor.transform.SetParent(party.GameObject.transform);
        }

        private void CameraMove(Vector3 old, Vector3 newPos)
        {
            if (IsActive(_tileCursor))
                Inactivate(_tileCursor);
            _selectedTile = null;
        }

        public ClientTile SelectedTile { get => _selectedTile; }

        private void ClickTile(ClientTile tile)
        {
            Log.Debug($"TileUI selecting tile {tile}");
            if (tile != null)
            {
                if (!IsActive(_tileCursor))
                    Activate(_tileCursor);
                MoveToTile(_tileCursor, tile);
                _selectedTile = tile;
            }
        }

        private void MoveToTile(GameObject cursor, Tile tile)
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

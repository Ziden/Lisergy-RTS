using Game.Tile;
using Game;
using GameAssets;
using UnityEngine;
using Assets.Code;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Events.Bus;
using Assets.Code.Assets.Code.Runtime;

/// <summary>
/// Listens for click events to draw the tile selection indicator
/// </summary>
public class IndicatorSelectedTileListener : IEventListener
{
    private TileEntity _selectedTile;
    private GameObject _tileCursor;
    private IGameClient _client;

    public IndicatorSelectedTileListener(IGameClient client)
    {
        _client = client;
        ClientState.OnSelectTile += ClickTile;
        ClientState.OnCameraMove += OnCameraMove;
        _client.ClientEvents.Register<EntityMovementRequestStarted>(this, OnStartMovement);
        _client.UnityServices().Assets.CreateMapObject(MapObjectPrefab.TileCursor, Vector3.zero, Quaternion.identity, o =>
        {
            o.SetActive(false);
            _tileCursor = o;
        });
    }

    public void OnStartMovement(EntityMovementRequestStarted ev)
    {
        if (IsActive(_tileCursor))
            Inactivate(_tileCursor);
        _selectedTile = null;
    }

    private void OnCameraMove(Vector3 newPos)
    {
        if (IsActive(_tileCursor))
            Inactivate(_tileCursor);
        _selectedTile = null;
    }

    public TileEntity SelectedTile { get => _selectedTile; }

    private void ClickTile(TileEntity tile)
    {
        if (tile == null) return;
        if (tile != null)
        {
            if (!IsActive(_tileCursor))
                Activate(_tileCursor);
            MoveToTile(_tileCursor, tile);
            _selectedTile = tile;
        }
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
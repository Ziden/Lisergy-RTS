using Game.Tile;
using GameAssets;
using UnityEngine;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.Events.Bus;
using Assets.Code.Assets.Code.Runtime;

/// <summary>
/// Listens for click events to draw the tile selection indicator
/// </summary>
public class IndicatorSelectedTileListener : IEventListener
{
    private TileModel _selectedTile;
    private GameObject _tileCursor;
    private IGameClient _client;

    public IndicatorSelectedTileListener(IGameClient client)
    {
        _client = client;
        ClientViewState.OnSelectTile += ClickTile;
        ClientViewState.OnCameraMove += OnCameraMove;
        _client.ClientEvents.On<EntityMovementRequestStarted>(this, OnStartMovement);
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

    public TileModel SelectedTile { get => _selectedTile; }

    private void ClickTile(TileModel tile)
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

    private void MoveToTile(GameObject cursor, TileModel tile)
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
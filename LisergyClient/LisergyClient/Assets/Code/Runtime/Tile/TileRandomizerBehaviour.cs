using Assets.Code;
using Assets.Code.Views;
using ClientSDK;
using Game.Tile;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Responsible for getting a tile and randomizing its decoration
/// Every tile is randomized based on its position as a random seed so the same tile will always look
/// on the client.
/// This is a 
/// </summary>
public class TileMonoComponent : MonoBehaviour
{
    private TileEntity _tile;

    /// <summary>
    /// Indicates if this tile was already decorated
    /// </summary>
    private bool _decorated;

    public List<GameObject> ChooseOne;
    public List<GameObject> Remove50;
    public List<GameObject> Remove85;
    public List<GameObject> Remove25;
    public List<GameObject> RandomizePosition;
    public List<GameObject> RandomizeRotation;

    // Gets removed if the tile is connected with the same tileid to east
    // TODO: Make this better in editor window perhaps ? (defining the target tile etc)
    public List<GameObject> RemoveWhenConnectEast;
    public List<GameObject> RemoveWhenConnectSouth;
    public List<GameObject> RemoveWhenConnectNorth;
    public List<GameObject> RemoveWhenConnectWest;

    public TileEntity Tile { get => _tile; private set => _tile = value; }

    /// <summary>
    /// Whenever two tiles of the same type are placed one near another they might need to connect and remove borders
    /// For instance when water connects with water they would remove the cliffs 
    /// </summary>
    private void ConnectToSameTileType(TileView thisTile, TileView otherTile, List<GameObject> toRemove)
    {
        if (otherTile.GameObject == null) return;
        var decorationComponent = thisTile.GameObject.GetComponent<TileMonoComponent>();
        var otherTileDecoration = otherTile.GameObject.GetComponent<TileMonoComponent>();
        toRemove.ForEach(e => Destroy(e));
        toRemove.Clear();
        if (!decorationComponent._decorated && otherTileDecoration._decorated)
        {
            DecorateBoundaries(otherTile);
        }
    }

    /// <summary>
    /// Decorate the borders of other tiles, connecting them 
    /// E.g when a water tile connects to another water tile and removes the cliffs from the
    /// borders of the tile
    /// </summary>
    private void DecorateBoundaries(TileView currentTileView)
    {
        var modules = ServiceContainer.Resolve<IServerModules> ();
        var tile = currentTileView.Entity;
        var decorationComponent = currentTileView.GameObject.GetComponent<TileMonoComponent>();
        var map = currentTileView.Entity.Chunk.Map;
        var northTile = map.GetTile(tile.X, tile.Y - 1);
        var southTile = map.GetTile(tile.X, tile.Y + 1);
        var eastTile = map.GetTile(tile.X + 1, tile.Y);
        var westTile = map.GetTile(tile.X - 1, tile.Y);
        TileView north = northTile == null ? null : (TileView)modules.Views.GetOrCreateView(northTile);
        TileView south = southTile == null ? null : (TileView)modules.Views.GetOrCreateView(southTile);
        TileView east = eastTile == null ? null : (TileView)modules.Views.GetOrCreateView(eastTile);
        TileView west = westTile == null ? null : (TileView)modules.Views.GetOrCreateView(westTile);

        if (decorationComponent.RemoveWhenConnectNorth.Count > 0 && (northTile != null && north.Entity.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, north, decorationComponent.RemoveWhenConnectNorth);
        }
        if (decorationComponent.RemoveWhenConnectSouth.Count > 0 && (southTile != null && south.Entity.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, south, decorationComponent.RemoveWhenConnectSouth);
        }
        if (decorationComponent.RemoveWhenConnectEast.Count > 0 && (eastTile != null && east.Entity.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, east, decorationComponent.RemoveWhenConnectEast);
        }

        if (decorationComponent.RemoveWhenConnectWest.Count > 0 && (westTile != null && west.Entity.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, west, decorationComponent.RemoveWhenConnectWest);
        }
    }

    public static int GetTilePositionHash(ushort a, ushort b)
    {
        var A = (uint)(a >= 0 ? 2 * (int)a : -2 * (int)a - 1);
        var B = (uint)(b >= 0 ? 2 * (int)b : -2 * (int)b - 1);
        var C = (uint)((A >= B ? A * A + A + B : A + B * B) / 2);
        return (int)(a < 0 && b < 0 || a >= 0 && b >= 0 ? C : -C - 1);
    }

    public void MakeHills()
    {
        var mesh = GetComponentInChildren<PlaneMesh>();
        if(mesh == null)
        {
            return;
        }
        var hillX = Random.Range(1, 4);
        var hillY = Random.Range(1, 4);
        mesh.Heights[hillX, hillY] = Random.value / 12;
        mesh.Heights[hillX + 1, hillY] = Random.value / 12;
        mesh.Heights[hillX, hillY + 1] = Random.value / 12;
        mesh.Heights[hillX + 1, hillY + 1] = Random.value / 12;

        if (Random.value > 0.5f)
        {
            hillX = Random.Range(1, 4);
            hillY = Random.Range(1, 4);
            mesh.Heights[hillX, hillY] = Random.value / 8;
            mesh.Heights[hillX + 1, hillY] = Random.value / 8;
            mesh.Heights[hillX, hillY + 1] = Random.value / 8;
            mesh.Heights[hillX + 1, hillY + 1] = Random.value / 8;
        }
        mesh.Adjust();
    }

    public void CreateTileDecoration(TileView tile)
    {
        Random.InitState(GetTilePositionHash(tile.Entity.X, tile.Entity.Y));
        _tile = tile.Entity;
        DecorateBoundaries(tile);

        _decorated = true;

        var removed = new List<GameObject>();
        foreach (var o in Remove50)
        {
            if (Random.value < 0.55)
                removed.Add(o);
        }
        foreach (var o in Remove85)
        {
            if (Random.value < 0.85)
                removed.Add(o);
        }
        foreach (var o in Remove25)
        {
            if (Random.value < 0.25)
                removed.Add(o);
        }

        if (ChooseOne != null && ChooseOne.Count > 0)
        {
            var one = ChooseOne[Random.Range(0, ChooseOne.Count)];
            foreach (var o in ChooseOne)
            {
                if (o != one)
                {
                    removed.Add(o);
                }
                else
                {
                    o.SetActive(true);
                }
            }
        }

        foreach (var rem in removed)
        {
            if (RandomizePosition != null)
                RandomizePosition.Remove(rem);
            if (RandomizeRotation != null)
                RandomizeRotation.Remove(rem);
            Destroy(rem);
        }

        foreach (var obj in RandomizePosition)
        {
            if (obj == null) continue;
            float x = -0.5f + Random.value;
            float y = -0.5f + Random.value;
            obj.transform.localPosition = new Vector3(x, 0, y);
            obj.transform.localRotation = Quaternion.Euler(0, Random.value * 360, 0);
        }

        foreach (var obj in RandomizeRotation)
        {
            if (obj == null) continue;
            obj.transform.Rotate(new Vector3(0, Random.value * 360, 0), Space.Self);
        }

        MakeHills();
    }
}

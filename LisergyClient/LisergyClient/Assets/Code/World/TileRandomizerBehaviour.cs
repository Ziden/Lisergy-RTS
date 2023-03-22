using Assets.Code;
using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.Tile;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizerBehaviour : MonoBehaviour
{
    private TileEntity _tile;

    public List<GameObject> Remove50;
    public List<GameObject> Remove85;
    public List<GameObject> Remove25;
    public List<GameObject> RandomizePosition;

    // Gets removed if the tile is connected with the same tileid to east
    // TODO: Make this better in editor window perhaps ? (defining the target tile etc)
    public List<GameObject> RemoveWhenConnectEast;
    public List<GameObject> RemoveWhenConnectSouth;
    public List<GameObject> RemoveWhenConnectNorth;
    public List<GameObject> RemoveWhenConnectWest;

    private static System.Random rnd = new System.Random();

    public TileEntity Tile { get => _tile; private set => _tile = value; }

    private static void DecorateBoundaries(TileView view)
    {
        var tile = view.Entity;
        var comp = view.GameObject.GetComponent<TileRandomizerBehaviour>();
        var map = view.Entity.Chunk.Map;
        var northTile = map.GetTile(tile.X, tile.Y - 1);
        var southTile = map.GetTile(tile.X, tile.Y + 1);
        var eastTile = map.GetTile(tile.X + 1, tile.Y);
        var westTile = map.GetTile(tile.X - 1, tile.Y);
        TileView north = northTile == null ? null : GameView.GetOrCreateTileView(northTile);
        TileView south = southTile == null ? null : GameView.GetOrCreateTileView(southTile);
        TileView east = eastTile == null ? null : GameView.GetOrCreateTileView(eastTile);
        TileView west = westTile == null ? null : GameView.GetOrCreateTileView(westTile);

        if (comp.RemoveWhenConnectNorth.Count > 0 && (northTile != null && north.Entity.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectNorth.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectNorth.Clear();
            if (!view.Decorated && north.Decorated)
            {
                DecorateBoundaries(north);
            }
        }
        if (comp.RemoveWhenConnectSouth.Count > 0 && (southTile != null && south.Entity.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectSouth.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectSouth.Clear();
            if (south != null && !view.Decorated && south.Decorated)
            {
                DecorateBoundaries(south);
            }
        }
        if (comp.RemoveWhenConnectEast.Count > 0 && (eastTile != null && east.Entity.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectEast.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectEast.Clear();
            if (east != null && !view.Decorated && east.Decorated)
            {
                DecorateBoundaries(east);
            }
        }

        if (comp.RemoveWhenConnectWest.Count > 0 && (westTile != null && west.Entity.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectWest.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectWest.Clear();
            if (west != null && !view.Decorated && west.Decorated)
            {
                DecorateBoundaries(west);
            }
        }
    }

    public void CreateTileDecoration(TileView tile)
    {
        _tile = tile.Entity;
        DecorateBoundaries(tile);
        tile.Decorated = true;

        var removed = new List<GameObject>();
        foreach (var o in Remove50)
        {
            if (rnd.NextDouble() < 0.55)
                removed.Add(o);
        }
        foreach (var o in Remove85)
        {
            if (rnd.NextDouble() < 0.85)
                removed.Add(o);
        }
        foreach (var o in Remove25)
        {
            if (rnd.NextDouble() < 0.25)
                removed.Add(o);
        }

        foreach (var rem in removed)
        {
            if (RandomizePosition != null)
                RandomizePosition.Remove(rem);
            Destroy(rem);
        }


        if (RandomizePosition == null || RandomizePosition.Count == 0)
            return;

        foreach (var obj in RandomizePosition)
        {

            float x = -0.5f + (float)rnd.NextDouble();
            float y = -0.5f + (float)rnd.NextDouble();
            obj.transform.localPosition = new Vector3(x, 0, y);

        }

    }
}

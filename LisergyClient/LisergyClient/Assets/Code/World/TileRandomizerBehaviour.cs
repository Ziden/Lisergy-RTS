using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizerBehaviour : MonoBehaviour
{
    private Tile _tile;

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

    public Tile Tile { get => _tile; private set => _tile = value; }

    private static void DecorateBoundaries(ClientTile tile)
    {

        var comp = tile.GetGameObject().GetComponent<TileRandomizerBehaviour>();

        var map = tile.Chunk.Map;
        var north = map.GetTile(tile.X, tile.Y - 1);
        var south = map.GetTile(tile.X, tile.Y + 1);
        var east = map.GetTile(tile.X + 1, tile.Y);
        var west = map.GetTile(tile.X - 1, tile.Y);

        if (comp.RemoveWhenConnectNorth.Count > 0 && (north != null && north.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectNorth.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectNorth.Clear();
            if (!tile.Decorated && ((ClientTile)north).Decorated)
            {
                DecorateBoundaries((ClientTile)north);
            }
        }
        if (comp.RemoveWhenConnectSouth.Count > 0 && (south != null && south.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectSouth.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectSouth.Clear();
            if (south != null && !tile.Decorated && ((ClientTile)south).Decorated)
            {
                DecorateBoundaries((ClientTile)south);
            }
        }
        if (comp.RemoveWhenConnectEast.Count > 0 && (east != null && east.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectEast.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectEast.Clear();
            if (east != null && !tile.Decorated && ((ClientTile)east).Decorated)
            {
                DecorateBoundaries((ClientTile)east);
            }
        }

        if (comp.RemoveWhenConnectWest.Count > 0 && (west != null && west.TileId == tile.TileId))
        {
            comp.RemoveWhenConnectWest.ForEach(e => Destroy(e));
            comp.RemoveWhenConnectWest.Clear();
            if (west != null && !tile.Decorated && ((ClientTile)west).Decorated)
            {
                DecorateBoundaries((ClientTile)west);
            }
        }
    }

    public void CreateTileDecoration(ClientTile tile)
    {
        _tile = tile;
        StackLog.Debug($"Decorating {tile}");
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

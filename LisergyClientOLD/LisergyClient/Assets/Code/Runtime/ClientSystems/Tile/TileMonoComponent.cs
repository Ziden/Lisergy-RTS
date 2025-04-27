using Assets.Code.Views;
using ClientSDK;
using Game.Engine.DataTypes;
using Game.Tile;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TileRandomizationSetup
{
    public List<GameObject> Objects = new List<GameObject>();
    public bool RandomizeLocation = false;
    public bool RandomizeRotation = false;
    public bool LeaveOne = false;
    public float ChanceToEnable = 1f;
}

/// <summary>
/// Responsible for getting a tile and randomizing its decoration
/// Every tile is randomized based on its position as a random seed so the same tile will always look
/// on the client.
/// This is a 
/// </summary>
public class TileMonoComponent : MonoBehaviour
{
    private static bool ENABLED = true;
    private static DeterministicRandom _rng = new DeterministicRandom();

    private TileModel _tile;

    public List<TileRandomizationSetup> _config = new List<TileRandomizationSetup>();

    /// <summary>
    /// Indicates if this tile was already decorated
    /// </summary>
    private bool _decorated;

    // Gets removed if the tile is connected with the same tileid to east
    // TODO: Make this better in editor window perhaps ? (defining the target tile etc)
    public List<GameObject> RemoveWhenConnectEast;
    public List<GameObject> RemoveWhenConnectSouth;
    public List<GameObject> RemoveWhenConnectNorth;
    public List<GameObject> RemoveWhenConnectWest;

    public TileModel Tile { get => _tile; private set => _tile = value; }

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
        if (!ENABLED) return;

        var modules = Assets.Code.UnityServicesContainer.Resolve<IServerModules>();
        var tile = currentTileView.Tile;
        var decorationComponent = currentTileView.GameObject.GetComponent<TileMonoComponent>();
        var map = currentTileView.Tile.Game.World;
        var northTile = map.GetTile(tile.X, tile.Y - 1);
        var southTile = map.GetTile(tile.X, tile.Y + 1);
        var eastTile = map.GetTile(tile.X + 1, tile.Y);
        var westTile = map.GetTile(tile.X - 1, tile.Y);
        TileView north = northTile == null ? null : (TileView)modules.Views.GetOrCreateView(northTile.Entity);
        TileView south = southTile == null ? null : (TileView)modules.Views.GetOrCreateView(southTile.Entity);
        TileView east = eastTile == null ? null : (TileView)modules.Views.GetOrCreateView(eastTile.Entity);
        TileView west = westTile == null ? null : (TileView)modules.Views.GetOrCreateView(westTile.Entity);

        if (decorationComponent.RemoveWhenConnectNorth.Count > 0 && (northTile != null && north.Tile.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, north, decorationComponent.RemoveWhenConnectNorth);
        }
        if (decorationComponent.RemoveWhenConnectSouth.Count > 0 && (southTile != null && south.Tile.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, south, decorationComponent.RemoveWhenConnectSouth);
        }
        if (decorationComponent.RemoveWhenConnectEast.Count > 0 && (eastTile != null && east.Tile.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, east, decorationComponent.RemoveWhenConnectEast);
        }

        if (decorationComponent.RemoveWhenConnectWest.Count > 0 && (westTile != null && west.Tile.SpecId == tile.SpecId))
        {
            ConnectToSameTileType(currentTileView, west, decorationComponent.RemoveWhenConnectWest);
        }
    }

    public static int GetTilePositionHash(ushort a, ushort b) => a * b * a * b * 31 * 31;


    // Need to change this if this code run async !
    private static HashSet<GameObject> _added = new HashSet<GameObject>();
    private static HashSet<GameObject> _removed = new HashSet<GameObject>();

    public void CreateTileDecoration(TileView tile)
    {
        unchecked
        {
            _rng.Reinitialise(GetTilePositionHash(tile.Tile.X, tile.Tile.Y));
            _tile = tile.Tile;
            DecorateBoundaries(tile);

            _decorated = true;

            if (!ENABLED)
            {
                foreach (var r in _config)
                {
                    foreach (var o in r.Objects)
                    {
                        Destroy(o);
                    }
                }
                return;
            }

            foreach (var r in _config)
            {
                if (r.LeaveOne)
                {
                    var chosen = r.Objects[_rng.Next(0, r.Objects.Count)];
                    for (var x = 0; x < r.Objects.Count; x++)
                    {
                        var o = r.Objects[x];
                        if (o != chosen) Destroy(o);
                    }
                    r.Objects.Clear();
                    r.Objects.Add(chosen);
                    chosen.SetActive(true);
                }

                foreach (var o in r.Objects)
                {
                    if (r.ChanceToEnable >= 1f || _rng.NextSingle() < r.ChanceToEnable)
                    {
                        _added.Add(o);
                        if (r.RandomizeLocation)
                        {
                            o.transform.localPosition = new Vector3(-0.5f + _rng.NextSingle(), 0, -0.5f + _rng.NextSingle());
                        }
                        if (r.RandomizeRotation)
                        {
                            o.transform.localRotation = Quaternion.Euler(0, _rng.NextSingle() * 360, 0);
                        }
                    }
                    else
                    {
                        _removed.Add(o);
                    }
                }

                foreach (var a in _added)
                {
                    a.gameObject.SetActive(true);
                }

                foreach (var rem in _removed)
                {
                    Destroy(rem);
                }

                _added.Clear();
                _removed.Clear();
            }
        }
    }
}

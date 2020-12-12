using Game.Events;
using Game.Events.ServerEvents;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    [Flags]
    public enum TileMetaFlags : byte
    {
        IS_OWNED = 1 << 0,
        HAS_BUILDING = 1 << 1,
        HAS_RESOURCE = 1 << 2,
        IS_UNEXPLORED = 1 << 3,
        HAS_UNITS = 1 << 4
    }

    [Flags]
    public enum TerrainData : byte
    {
        FOREST = 1 << 0,
        MOUNTAIN = 1 << 1,
        WATER = 1 << 2,
        DESERT = 1 << 3,
        RIVER = 1 << 4,
        BUSHES = 1 << 5,
        HILL = 1 << 6,
        UNUSED = 1 << 7
    }

    [Flags]
    public enum TileDeltaFlags : byte
    {
        BUILDING = 1 << 0,
        OWNER = 1 << 1,
        VISIBILITY = 1 << 2,
        B = 1 << 3,
        C = 1 << 4,
        D = 1 << 5,
        E = 1 << 6,
        F = 1 << 7
    }

    [Serializable]
    public class Tile
    {
        private byte _terrainData;
        private byte _resourceData;
        private byte _buildingID;
        private string _userId;
        private ushort _y;
        private ushort _x;
        private byte _flags;

        public Tile(Chunk c, int x, int y)
        {
            this.Chunk = c;
            this.X = (ushort)x;
            this.X = (ushort)y;
        }

        [NonSerialized]
        private Chunk _chunk;

        [NonSerialized]
        private List<Unit> _units = new List<Unit>();

        [NonSerialized]
        private HashSet<PlayerEntity> _visibleTo = new HashSet<PlayerEntity>();

        [NonSerialized]
        private HashSet<WorldEntity> _viewing = new HashSet<WorldEntity>();

        [NonSerialized]
        private Building _building;

        [NonSerialized]
        private PlayerEntity _owner;

        public string UserID { get => _userId; }
        public byte BuildingID { get => _buildingID; }
        public ushort Y { get => _y; private set => _y = value; }
        public ushort X { get => _x; private set => _x = value; }
        public byte TerrainData { get => _terrainData; set => _terrainData = value; }
        public byte ResourceData { get => _resourceData; set => _resourceData = value; }
        public Chunk Chunk { get => _chunk; set => _chunk = value; }

        public Building Building
        {
            get => _building; set
            {
                if (value != null)
                {
                    var buildingSpec = value.GetSpec();
                    Owner = value.Owner;
                    Owner.Buildings.Add(value);
                    _buildingID = value.BuildingID;
                    _flags.AddFlag(TileMetaFlags.HAS_BUILDING);
                    foreach(var tile in GetAOE(buildingSpec.LOS))
                    {
                        tile.SetSeenBy(value);
                    }
                }
                else
                {
                    if(_building != null)
                    {
                        var spec = _building.GetSpec();
                        foreach (var tile in GetAOE(spec.LOS))
                        {
                            tile.SetUnseenBy(_building);
                        }
                        Owner.Buildings.Remove(_building);
                    }
                    _buildingID = 0;
                    _flags.RemoveFlag(TileMetaFlags.HAS_BUILDING);
                    Owner = null;
                }
                _building = value;
            }
        }

        public PlayerEntity Owner
        {
            get => _owner; set
            {
                
                if (value != null)
                {
                    _userId = value.UserID;
                    _flags.AddFlag(TileMetaFlags.IS_OWNED);
                }
                else
                {
                    _userId = null;
                    _flags.RemoveFlag(TileMetaFlags.IS_OWNED);
                }
                _owner = value;
            }
        }

        public byte Flags { get => _flags; set => _flags = value; }

        public void SetSeenBy(WorldEntity entity)
        {
            _viewing.Add(entity);
            if (_visibleTo.Add(entity.Owner))
            {
                EventSink.TileVisible(new TileVisibleEvent()
                {
                    Tile = this,
                    Viewer = entity
                });
            }
        }

        public void SetUnseenBy(WorldEntity entity)
        {
            _viewing.Remove(entity);
            if (!_viewing.Any(e => e.Owner == entity.Owner)) 
                _visibleTo.Remove(entity.Owner);
        }

        public bool IsVisibleTo(PlayerEntity player)
        {
            return _visibleTo.Contains(player);
        }

        public override string ToString()
        {
            return $"<Tile {X}-{Y}>";
        }

        public IEnumerable<Tile> GetAOE(ushort radius)
        {
            var x = Y;
            var y = Y;
            for (var xx = x - radius; xx <= x + radius; xx++)
            {
                for (var yy = y - radius; yy <= y + radius; yy++)
                {
                    if (Chunk.World.ValidCoords(xx, yy))
                        yield return Chunk.World.GetTile(xx, yy);
                }
            }
        }

        public bool SameData(Tile other)
        {
            return this.BuildingID == other.BuildingID && this.X == other.X && this.Y == other.Y && this._userId == other._userId && this._units == other._units;
        }
    }
}

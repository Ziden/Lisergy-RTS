using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
using Game.World;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    [Serializable]
    public class Tile
    {
        private byte _resourceID;
        private byte _tileId;
        private byte _buildingID;
        private string _userId;
        private ushort _y;
        private ushort _x;

        public Tile(Chunk c, int x, int y)
        {
            this._chunk = c;
            this._x = (ushort)x;
            this._y = (ushort)y;
        }

        [NonSerialized]
        private Chunk _chunk;

        [NonSerialized]
        private List<Party> _parties = new List<Party>();

        [NonSerialized]
        protected HashSet<PlayerEntity> _visibleTo = new HashSet<PlayerEntity>();

        [NonSerialized]
        protected HashSet<WorldEntity> _viewing = new HashSet<WorldEntity>();

        [NonSerialized]
        private Building _building;
       
        [NonSerialized]
        private PlayerEntity _owner;

        public TileSpec GetSpec()
        {
            return StrategyGame.Specs.Tiles[this.TileId];
        }

        public virtual string UserID { get => _userId; }
        public virtual byte BuildingID { get => _buildingID; }
        public virtual ushort Y { get => _y; }
        public virtual ushort X { get => _x; }
        public virtual HashSet<WorldEntity> Viewing { get { return _viewing; } }
        public virtual Chunk Chunk { get { return _chunk; } }
        public virtual byte TileId { get => _tileId; set => _tileId = value; }
        public virtual byte ResourceID { get => _resourceID; set => _resourceID = value; }
        public virtual List<Party> Parties { get { return _parties; } }

        public virtual void TeleportParty(Party party)
        {
            var los = party.GetLineOfSight();
            var previousTile = party.Tile;
            if (previousTile != null)
            {
                previousTile._parties.Remove(party);
                foreach (var tile in previousTile.GetAOE(los))
                    tile.SetUnseenBy(party);
            }
            _parties.Add(party);
            party.Tile = this;

            foreach (var tile in GetAOE(los))
                tile.SetSeenBy(party);
            foreach(var viewer in _viewing)
            {
                EventSink.PartyVisible(new PartyVisibleEvent()
                {
                    Viewer = viewer,
                    Party = party
                });
            }
        }

        public virtual Building Building
        {
            get => _building; set
            {
                if (value != null)
                {
                    value.Tile = this;
                    var buildingSpec = value.GetSpec();
                    Owner = value.Owner;
                    Owner.Buildings.Add(value);
                    this.Chunk.Buildings.Add(value);
                    _buildingID = value.BuildingID;
                    Log.Debug($"Expanding +{buildingSpec.LOS} LOS on {this} for {Owner}");
                    foreach (var tile in GetAOE(buildingSpec.LOS))
                        tile.SetSeenBy(value);
                }
                else
                {
                    if (_building != null)
                    {
                        var spec = _building.GetSpec();
                        foreach (var tile in GetAOE(spec.LOS))
                            tile.SetUnseenBy(_building);
                        this.Chunk.Buildings.Remove(_building);
                    }
                    _buildingID = 0;
                    Owner = null;
                }
                _building = value;
            }
        }

        public virtual PlayerEntity Owner
        {
            get => _owner; set
            {
                if (value != null)
                {
                    _userId = value.UserID;
                }
                else
                {
                    _userId = null;
                }
                _owner = value;
            }
        }

        public virtual void SetSeenBy(WorldEntity entity)
        {
            _viewing.Add(entity);
            if (_visibleTo.Add(entity.Owner))
            {
                entity.Owner.VisibleTiles.Add(this);
                EventSink.TileVisible(new TileVisibleEvent()
                {
                    Tile = this,
                    Viewer = entity
                });
                Parties.ForEach(u => EventSink.PartyVisible(new PartyVisibleEvent()
                {
                    Viewer = entity,
                    Party = u
                }));
            }
        }

        public virtual Tile GetNeighbor(Direction d)
        {
            switch (d)
            {
                case Direction.EAST: return _chunk.ChunkMap.GetTile(X + 1, Y);
                case Direction.WEST: return _chunk.ChunkMap.GetTile(X - 1, Y);
                case Direction.SOUTH: return _chunk.ChunkMap.GetTile(X - 1, Y);
                case Direction.NORTH: return _chunk.ChunkMap.GetTile(X + 1, Y);
            }
            return null;
        }

        public virtual void SetUnseenBy(WorldEntity entity)
        {
            _viewing.Remove(entity);
            if (!_viewing.Any(e => e.Owner == entity.Owner))
            {
                entity.Owner.VisibleTiles.Remove(this);
                _visibleTo.Remove(entity.Owner);
            }
        }

        public virtual bool IsVisibleTo(PlayerEntity player)
        {
            return _visibleTo.Contains(player);
        }

        public IEnumerable<Tile> GetAOE(ushort radius)
        {
            for (var xx = X - radius; xx <= X + radius; xx++)
            {
                for (var yy = Y - radius; yy <= Y + radius; yy++)
                {
                    if (Chunk.ChunkMap.ValidCoords(xx, yy))
                        yield return Chunk.ChunkMap.GetTile(xx, yy);
                }
            }
        }

        public override string ToString()
        {
            return $"<Tile {X}-{Y} ID={TileId} Res={ResourceID} B={BuildingID}>";
        }

        public bool Passable
        {
            get => MovementFactor > 0;
        }

        public float MovementFactor { get => GetSpec().MovementFactor; }

    }
}

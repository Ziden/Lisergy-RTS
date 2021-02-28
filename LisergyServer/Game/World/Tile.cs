using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
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

        public virtual TileSpec Spec { get => StrategyGame.Specs.Tiles[this.TileId]; }
        public virtual GameWorld World { get => Chunk.ChunkMap.World; }
        public virtual string UserID { get => _userId; }
        public virtual byte BuildingID { get => _buildingID; }
        public virtual ushort Y { get => _y; }
        public virtual ushort X { get => _x; }
        public virtual HashSet<WorldEntity> Viewing { get { return _viewing; } }
        public virtual Chunk Chunk { get { return _chunk; } }
        public virtual byte TileId { get => _tileId; set => _tileId = value; }
        public virtual byte ResourceID { get => _resourceID; set => _resourceID = value; }
        public virtual List<Party> Parties { get { return _parties; }}

        public virtual Building Building
        {
            get => _building; set
            {
                _building = value;
                if (value != null)
                {
                    value.Tile = this;
                    Owner = value.Owner;
                    _buildingID = value.SpecID;
                   
                }
                else
                {
                    _buildingID = 0;
                    Owner = null;
                }
            }
        }

        public virtual PlayerEntity Owner
        {
            get => _owner; set
            {
                if (value != null)
                    _userId = value.UserID;
                else
                    _userId = null;
                _owner = value;
            }
        }

        #region FOG of war
        public virtual void SetSeenBy(ExploringEntity entity)
        {
            _viewing.Add(entity);
            if (_visibleTo.Add(entity.Owner))
            {
                entity.Owner.VisibleTiles.Add(this);
                entity.Owner.Send(new TileVisibleEvent(this));
                Parties.ForEach(party =>
                    entity.Owner.Send(new EntityVisibleEvent(party, entity))
                );
            }
        }

        public virtual void SetUnseenBy(ExploringEntity entity)
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
                for (var yy = Y - radius; yy <= Y + radius; yy++)
                    if (Chunk.ChunkMap.ValidCoords(xx, yy))
                        yield return Chunk.ChunkMap.GetTile(xx, yy);

        }
        #endregion

        public bool Passable
        {
            get => MovementFactor > 0;
        }

        public float MovementFactor { get => Spec.MovementFactor; }

        public override string ToString()
        {
            return $"<Tile {X}-{Y} ID={TileId} Res={ResourceID} B={BuildingID}>";
        }
    }
}

using Game.Entity;
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

        public virtual TileSpec Spec { get => StrategyGame.Specs.Tiles[this.TileId]; }
        public virtual GameWorld World { get => Chunk.ChunkMap.World; }
        public virtual string OwnerID { get => _building?.OwnerID; }
        public virtual ushort Y { get => _y; }
        public virtual ushort X { get => _x; }
        public virtual HashSet<WorldEntity> Viewing { get { return _viewing; } }
        public virtual Chunk Chunk { get { return _chunk; } }
        public virtual byte TileId { get => _tileId; set => _tileId = value; }
        public virtual byte ResourceID { get => _resourceID; set => _resourceID = value; }
        public virtual List<Party> Parties { get { return _parties; }}

        public virtual Building Building
        {
            get => _building;
            set
            {
                if(value == null && _building != null)
                    _building.Tile = null;
                _building = value;
                if (value != null)
                    value.Tile = this;
            }
        }

        #region FOG of war
        public virtual void SetSeenBy(ExploringEntity entity)
        {
            _viewing.Add(entity);
            if (_visibleTo.Add(entity.Owner))
            {
                entity.Owner.VisibleTiles.Add(this);
                SendTileInformation(entity.Owner, entity);
            }
        }

        public void SendTileInformation(PlayerEntity player, ExploringEntity viewer)
        {
            player.Send(new TileVisibleEvent(this));
            foreach(var party in Parties)
                if (party != viewer)
                    player.Send(new EntityVisibleEvent(party));

            if (Building != null && viewer != Building)
                player.Send(new EntityVisibleEvent(Building));
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
            return $"<Tile {X}-{Y} ID={TileId} Res={ResourceID} Building={Building?.SpecID}>";
        }
    }
}

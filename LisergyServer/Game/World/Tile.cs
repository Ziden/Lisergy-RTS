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
        protected HashSet<PlayerEntity> _playersViewing = new HashSet<PlayerEntity>();

        [NonSerialized]
        protected HashSet<WorldEntity> _entitiesViewing = new HashSet<WorldEntity>();

        [NonSerialized]
        private Building _building;

        public virtual TileSpec Spec { get => StrategyGame.Specs.Tiles[this.TileId]; }
        public virtual GameWorld World { get => Chunk.ChunkMap.World; }
        public virtual string OwnerID { get => _building?.OwnerID; }
        public virtual ushort Y { get => _y; }
        public virtual ushort X { get => _x; } 
        public virtual HashSet<WorldEntity> EntitiesViewing { get { return _entitiesViewing; } }
        public virtual Chunk Chunk { get { return _chunk; } }
        public virtual byte TileId { get => _tileId; set => _tileId = value; }
        public virtual byte ResourceID { get => _resourceID; set => _resourceID = value; }
        public virtual List<Party> Parties { get { return _parties; }}
        public virtual HashSet<PlayerEntity> PlayersViewing { get => _playersViewing; }

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

        public virtual void SetSeenBy(ExploringEntity explorer)
        {
            _entitiesViewing.Add(explorer);
            if (_playersViewing.Add(explorer.Owner))
            {
                explorer.Owner.OnceExplored.Add(this);
                explorer.Owner.VisibleTiles.Add(this);
                SendTileInformation(explorer.Owner, explorer);
            }
        }

        public virtual void SetUnseenBy(ExploringEntity unexplorer)
        {
            _entitiesViewing.Remove(unexplorer);
            if (!_entitiesViewing.Any(e => e.Owner == unexplorer.Owner))
            {
                unexplorer.Owner.VisibleTiles.Remove(this);
                _playersViewing.Remove(unexplorer.Owner);
            }
        }

        public void SendTileInformation(PlayerEntity player, ExploringEntity viewer)
        {
            player.Send(new TileVisibleEvent(this));
            foreach (var party in Parties)
                if (party != viewer)
                    player.Send(new EntityVisibleEvent(party));

            if (Building != null && viewer != Building)
                player.Send(new EntityVisibleEvent(Building));
        }

        public virtual bool IsVisibleTo(PlayerEntity player)
        {
            return _playersViewing.Contains(player);
        }

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

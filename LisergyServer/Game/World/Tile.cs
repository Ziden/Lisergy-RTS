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
        private List<MovableWorldEntity> _parties = new List<MovableWorldEntity>();

        [NonSerialized]
        protected HashSet<PlayerEntity> _playersViewing = new HashSet<PlayerEntity>();

        [NonSerialized]
        protected HashSet<WorldEntity> _entitiesViewing = new HashSet<WorldEntity>();

        [NonSerialized]
        private StaticEntity _staticEntity;

        public virtual TileSpec Spec { get => StrategyGame.Specs.Tiles[this.TileId]; }
        public virtual string OwnerID { get => _staticEntity?.OwnerID; }
        public virtual ushort Y { get => _y; }
        public virtual ushort X { get => _x; }
        public virtual HashSet<WorldEntity> EntitiesViewing { get { return _entitiesViewing; } }
        public virtual Chunk Chunk { get { return _chunk; } }
        public virtual byte TileId { get => _tileId; set => _tileId = value; }
        public virtual byte ResourceID { get => _resourceID; set => _resourceID = value; }
        public virtual List<MovableWorldEntity> MovingEntities { get { return _parties; } }
        public virtual HashSet<PlayerEntity> PlayersViewing { get => _playersViewing; }
        public float MovementFactor { get => Spec.MovementFactor; }

        public StrategyGame Game => Chunk.Map.World.Game;
        // TODO, Remove Setter for StaticEntity (use entity Tile setter)
        public virtual StaticEntity StaticEntity { get => _staticEntity; set => _staticEntity = value; }

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
            // send movable entities in tile
            foreach (var movingEntity in MovingEntities)
                if (movingEntity != viewer)
                    player.Send(new EntityVisibleEvent(movingEntity));
            // send static entity in tile
            if (StaticEntity != null && viewer != StaticEntity)
                player.Send(new EntityVisibleEvent(StaticEntity));
        }

        public virtual bool IsVisibleTo(PlayerEntity player)
        {
            return _playersViewing.Contains(player);
        }

        public bool Passable
        {
            get => MovementFactor > 0;
        }

        public override string ToString()
        {
            return $"<Tile {X}-{Y} ID={TileId} " +
                (ResourceID == 0 ? "" : $"Res={ResourceID}") +
                (StaticEntity == null ? "" : $"Building={StaticEntity?.ToString()}") +
                ">";
        }
    }
}

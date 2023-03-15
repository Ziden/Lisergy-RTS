using Game.Entity;
using Game.Events;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.World.Components;
using System;
using System.Collections.Generic;

namespace Game
{

    [Serializable]
    public partial class WorldEntity : Ownable, IDeltaTrackable
    {
        protected static Gaia Gaia { get; private set; } = new Gaia();

        protected GameId _id;
        protected ushort _x;
        protected ushort _y;

        [NonSerialized]
        protected Tile _tile;

        [NonSerialized]
        protected Tile _previousTile;

        public WorldEntity(PlayerEntity owner) : base(owner)
        {
            _id = Guid.NewGuid();
            DeltaFlags = new DeltaFlags(this);
        }

        public bool IsDestroyed => _tile != null;

        public virtual GameId Id { get => _id; set => _id = value; }
        public virtual ushort X { get => _x; }
        public virtual ushort Y { get => _y; }

        public virtual Tile Tile
        {
            get => _tile; set
            {
                _previousTile = _tile;
                _tile = value;

                if (_previousTile == null || _tile == null)
                {
                    DeltaFlags.SetFlag(DeltaFlag.EXISTENCE);
                }
                else if (_previousTile != _tile)
                {
                    DeltaFlags.SetFlag(DeltaFlag.POSITION);
                }

                if (_tile != null)
                {
                    _x = _tile.X;
                    _y = _tile.Y;
                } else
                {
                    _x = 0;
                    _y = 0;
                    if(_previousTile != null)
                    {
                        foreach (var viewer in _previousTile.GetComponent<TileVisibilityComponent>().PlayersViewing)
                            viewer.Send(new EntityDestroyPacket(this));
                    }
                }
               

                Log.Info($"Placed {this} in {_tile}");
            }
        }
    }
}

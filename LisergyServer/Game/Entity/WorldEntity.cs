using Game.DataTypes;
using Game.ECS;
using Game.Entity;
using Game.Entity.Entities;
using Game.Events;
using Game.Events.GameEvents;
using Game.World.Components;
using System;

namespace Game
{

    [Serializable]
    public partial class WorldEntity : IOwnable, IMapEntity
    {
        protected static Gaia Gaia { get; private set; } = new Gaia();

        protected GameId _id;
        protected ushort _x;
        protected ushort _y;
        private GameId _ownerId;

        [NonSerialized]
        private PlayerEntity _owner;
        [field: NonSerialized]
        public ComponentSet _components { get; private set; }
        [NonSerialized]
        protected Tile _tile;
        [NonSerialized]
        protected Tile _previousTile;

        public WorldEntity(PlayerEntity owner)
        {
            Owner = owner;
            _id = Guid.NewGuid();
            DeltaFlags = new DeltaFlags(this);
            _components = new ComponentSet(this);
        }

        public bool IsInMap => _tile != null;

        public virtual GameId Id { get => _id; set => _id = value; }
        public virtual ushort X { get => _x; }
        public virtual ushort Y { get => _y; }
        public IComponentSet Components => _components;

        public GameId EntityId => _id;

        public GameId OwnerID { get => _ownerId; }

        public virtual PlayerEntity Owner
        {
            get => _owner; set
            {
                if (value != null)
                    _ownerId = value.UserID;
                else
                    _ownerId = GameId.ZERO;
                _owner = value;
            }
        }

        public Tile PreviousTile => _previousTile;

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
              
                if (_previousTile != null)
                {
                    var moveOut = new EntityMoveOutEvent()
                    {
                        Entity = this,
                        ToTile = value,
                        FromTile = _previousTile
                    };
                    _previousTile.Components.CallEvent(moveOut);
                    this.Components.CallEvent(moveOut);
                }
                if (value != null)
                {
                    var moveIn = new EntityMoveInEvent()
                    {
                        Entity = this,
                        ToTile = _tile,
                        FromTile = _previousTile
                    };
                    value.Components.CallEvent(moveIn);
                    this.Components.CallEvent(moveIn);
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
                        foreach (var viewer in _previousTile.Components.Get<TileVisibility>().PlayersViewing)
                            viewer.Send(new EntityDestroyPacket(this));
                    }
                }
                Log.Info($"Moved {this} to {_tile}");
            }
        }
    }
}

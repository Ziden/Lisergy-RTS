using Game.Battle;
using Game.Entity;
using Game.Events;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using System;

namespace Game
{
    [Serializable]
    public class WorldEntity : Ownable
    {
        protected static Gaia Gaia { get; private set; } = new Gaia();

        protected string _id;
        protected ushort _x;
        protected ushort _y;

        [NonSerialized]
        protected Tile _tile;

        public WorldEntity(PlayerEntity owner) : base(owner)
        {
            _id = Guid.NewGuid().ToString();
        }

        public virtual string Id { get => _id; set => _id = value; }
        public virtual int X { get => _x; }
        public virtual int Y { get => _y; }

        protected string _battleID;

        public virtual string BattleID { get => _battleID; 
            set { 
                _battleID = value;
            } 
        }

        public bool IsBattling => _battleID != null;

        public virtual Tile Tile
        {
            get => _tile; set
            {
                if (value != null)
                {
                    value.Game?.GameEvents.Call(new EntityMoveEvent()
                    {
                        Entity = this,
                        NewTile = value,
                        OldTile = _tile
                    });
                }
                var oldTile = _tile;
                _tile = value;
                if(_tile != null)
                {
                    _x = _tile.X;
                    _y = _tile.Y;
                } else
                {
                    _x = 0;
                    _y = 0;
                    if(oldTile != null)
                    {
                        foreach (var viewer in oldTile.PlayersViewing)
                            viewer.Send(new EntityDestroyPacket(this));
                    }
                }
                Log.Debug($"{this} placed in {value}");
            }
        }
    }
}

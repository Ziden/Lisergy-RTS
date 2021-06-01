using Game.Entity;
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

        public bool IsBattling => _battleID != null;

        public virtual Tile Tile
        {
            get => _tile; set
            {
                if (_tile == null && value != null)
                {
                    foreach (var viewer in value.EntitiesViewing)
                    {
                        Log.Info($"New entity placed {this}, sending visibility");
                        //viewer.Owner.Send(new EntityVisibleEvent(this));
                    }
                }    
                _tile = value;
                _x = _tile.X;
                _y = _tile.Y;
                Log.Debug($"{this} placed in {value}");
            }
        }
    }
}

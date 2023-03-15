using Game.Entity;
using Game.Events;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.World.Components;
using System;

namespace Game
{
    [Serializable]
    public class WorldEntity : Ownable
    {
        protected static Gaia Gaia { get; private set; } = new Gaia();

        protected GameId _id;
        protected ushort _x;
        protected ushort _y;

        [NonSerialized]
        protected Tile _tile;

        public WorldEntity(PlayerEntity owner) : base(owner)
        {
            _id = Guid.NewGuid().ToString();
        }

        public bool IsDestroyed => _tile != null;

        public virtual GameId Id { get => _id; set => _id = value; }
        public virtual ushort X { get => _x; }
        public virtual ushort Y { get => _y; }

        public virtual Tile Tile
        {
            get => _tile; set
            {
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
                        foreach (var viewer in oldTile.GetComponent<TileVisibilityComponent>().PlayersViewing)
                            viewer.Send(new EntityDestroyPacket(this));
                    }
                }
               

                Log.Info($"Placed {this} in {_tile}");
            }
        }
    }
}

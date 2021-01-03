using System;

namespace Game
{
    [Serializable]
    public class WorldEntity : Ownable
    {
        protected string _id;
        protected ushort _x;
        protected ushort _y;

        public WorldEntity(PlayerEntity owner): base(owner){ }

        public virtual string Id { get => _id; set => _id = value; }
        public virtual int X { get => _x; }
        public virtual int Y { get => _y; }
 
        public virtual Tile Tile { get => _tile; set {
                _tile = value;
                _x = _tile.X;
                _y = _tile.Y;
                Log.Debug("Updated unit tile");
            }
        }
    
        [NonSerialized]
        protected Tile _tile;
    } 
}

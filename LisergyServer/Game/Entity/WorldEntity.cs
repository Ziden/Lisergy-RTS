using System;

namespace Game
{
    [Serializable]
    public class WorldEntity
    {
        protected string _ownerId;
        protected string _id;
        protected ushort _x;
        protected ushort _y;

        public WorldEntity(PlayerEntity owner)
        {
            this.Owner = owner;
        }

        public string Id { get => _id; set => _id = value; }
        public int X { get => _x; }
        public int Y { get => _y; }
        public string OwnerID { get => _ownerId; }
        public virtual Tile Tile { get => _tile; set {
                _tile = value;
                _x = _tile.X;
                _y = _tile.Y;
                Log.Debug("Updated unit tile");
            }
        }

        public virtual PlayerEntity Owner { get => _owner; set
            {
                if (value != null)
                    _ownerId = value.UserID;
                else
                    _ownerId = null;
                _owner = value;
            }
        }

        [NonSerialized]
        private PlayerEntity _owner;

        [NonSerialized]
        protected Tile _tile;

     
    } 
}

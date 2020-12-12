using System;

namespace Game
{
    [Serializable]
    public class WorldEntity
    {
        private string _ownerId;
        private string _id;
        private int _x;
        private int _y;

        public string Id { get => _id; set => _id = value; }
        public int X { get => _x; }
        public int Y { get => _y; }
        public Tile Tile { get => _tile; set {
                _tile = value;
                _x = _tile.X;
                _y = _tile.Y;
            }
        }

        public PlayerEntity Owner { get => _owner; set
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
        private Tile _tile;

        public WorldEntity(PlayerEntity owner)
        {
            this.Owner = owner;
        }
    } 
}

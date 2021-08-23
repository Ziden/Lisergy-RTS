using Game.Entity;
using Game.Events;
using GameData;
using System;

namespace Game
{
    [Serializable]
    public class StaticEntity : ExploringEntity
    {
        public override byte GetLineOfSight()
        {
            return 0;
        }

        public override Tile Tile
        {
            get => base.Tile; set
            {
                var newTile = value;
                var oldTile = base.Tile;
                base.Tile = newTile;

                if (newTile != null)
                    newTile.StaticEntity = this;
                else if(oldTile != null)
                {
                    oldTile.StaticEntity = null;
                }
            }
        }

        public StaticEntity(PlayerEntity owner) : base(owner)
        {
        }
    }
}

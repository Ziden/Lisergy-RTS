using Game.Entity;
using Game.Events;
using Game.Events.GameEvents;
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
                    newTile.CallEvent(new StaticEntityPlacedEvent(this, newTile));
                else if(oldTile != null)
                {
                    oldTile.CallEvent(new StaticEntityRemovedEvent(this, oldTile));
                }
            }
        }

        public StaticEntity(PlayerEntity owner) : base(owner)
        {
        }
    }
}

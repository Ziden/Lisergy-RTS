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
                if (newTile != null)
                    newTile.CallComponentEvents(new StaticEntityPlacedEvent(this, newTile));
                else if(oldTile != null)
                {
                    oldTile.CallComponentEvents(new StaticEntityRemovedEvent(this, oldTile));
                }
                base.Tile = newTile;
            }
        }

        public StaticEntity(PlayerEntity owner) : base(owner)
        {
        }
    }
}

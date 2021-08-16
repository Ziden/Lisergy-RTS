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
                if (value != null)
                    value.StaticEntity = this;
                else if(base.Tile != null)
                {
                    base.Tile.StaticEntity = null;
                    foreach (var viewer in base.Tile.PlayersViewing)
                        viewer.Send(new EntityDestroyEvent(this));
                }
                base.Tile = value;
            }
        }

        public StaticEntity(PlayerEntity owner) : base(owner)
        {
        }
    }
}

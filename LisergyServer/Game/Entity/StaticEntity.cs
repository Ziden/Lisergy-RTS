using Game.Entity;
using Game.Events.GameEvents;
using Game.World.Components;
using System;

namespace Game
{

    public enum BuildingSpecType : byte
    {
        DUNGEON, 
        CONSTRUCTION
    }

    [Serializable]
    public class BuildingEntity : WorldEntity
    {
        public ushort SpecID => Components.Get<PlayerBuildingComponent>().SpecId;

        public override Tile Tile
        {
            get => base.Tile; set
            {
                var newTile = value;
                var oldTile = base.Tile;
                if (newTile != null)
                    newTile.Components.CallEvent(new BuildingPlacedEvent(this, newTile));
                else if(oldTile != null)
                {
                    oldTile.Components.CallEvent(new BuildingRemovedEvent(this, oldTile));
                }
                base.Tile = newTile;
            }
        }

        public BuildingEntity(PlayerEntity owner) : base(owner)
        {
        }
    }
}

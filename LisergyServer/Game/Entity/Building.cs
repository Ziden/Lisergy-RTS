using Game.Entity;
using GameData;
using System;

namespace Game
{
    [Serializable]
    public class Building : ExploringEntity
    {
        public byte SpecID { get; private set; }

        public override byte GetLineOfSight()
        {
            return GetSpec().LOS;
        }

        public Building(byte id, PlayerEntity owner): base(owner)
        {
            this.SpecID = id;
        }

        public override Tile Tile
        {
            get { return base.Tile; }
            set
            {
                if (value.StaticEntity != this)
                    throw new Exception("Use tile.Building = value to set a building into a tile");
                base.Tile = value;
            }
        }

        public BuildingSpec GetSpec()
        {
            return StrategyGame.Specs.Buildings[SpecID];
        }

        public override string ToString()
        {
            return $"<Building x={_x} y={_y} id={SpecID} Owner={Owner}>";
        }
    }
}

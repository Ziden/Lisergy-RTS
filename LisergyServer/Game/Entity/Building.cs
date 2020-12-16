using GameData;
using System;

namespace Game
{
    public class Building : WorldEntity
    {
        public byte BuildingID { get; private set; }
        
        public Building(byte id, PlayerEntity owner): base(owner)
        {
            this.BuildingID = id;
        }

        public BuildingSpec GetSpec()
        {
            return StrategyGame.Specs.Buildings[BuildingID];
        }

        public override string ToString()
        {
            return $"<Building tile={Tile} id={BuildingID} Owner={Owner}>";
        }
    }
}

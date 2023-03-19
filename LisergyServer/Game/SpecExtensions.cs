using GameData;
using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public static class SpecExtensions 
    {
        public static UnitSpec GetSpec(this Unit o) => StrategyGame.Specs.Units[o.SpecId];

        public static BuildingSpec GetSpec(this BuildingEntity o) => StrategyGame.Specs.Buildings[o.SpecID];

        public static TileSpec GetSpec(this Tile o) => StrategyGame.Specs.Tiles[o.TileId];
    }

}

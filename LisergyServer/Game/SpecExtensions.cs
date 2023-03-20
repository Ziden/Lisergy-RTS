using Game.Battler;
using Game.Building;
using Game.Tile;
using GameData;
using GameData.Specs;

namespace Game
{
    public static class SpecExtensions
    {
        public static UnitSpec GetSpec(this Unit o)
        {
            return StrategyGame.Specs.Units[o.SpecId];
        }

        public static BuildingSpec GetSpec(this PlayerBuildingEntity o)
        {
            return StrategyGame.Specs.Buildings[o.SpecID];
        }

        public static TileSpec GetSpec(this TileEntity o)
        {
            return StrategyGame.Specs.Tiles[o.TileId];
        }
    }

}

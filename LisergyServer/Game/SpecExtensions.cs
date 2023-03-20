using Game.Battler;
using Game.Building;
using Game.Tile;
using GameData;
using GameData.Specs;

namespace Game
{
    public static class SpecExtensions
    {
        public static UnitSpec GetSpec(this Unit o) => StrategyGame.Specs.Units[o.SpecId];

        public static BuildingSpec GetSpec(this PlayerBuildingEntity o) => StrategyGame.Specs.Buildings[o.SpecID];

        public static TileSpec GetSpec(this TileEntity o) => StrategyGame.Specs.Tiles[o.TileId];
    }

}

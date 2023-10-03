using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Tile;
using GameData;
using GameData.Specs;

namespace Game
{
    public static class SpecExtensions
    {
        public static UnitSpec GetSpec(this Unit o)
        {
            return GameLogic.Specs.Units[o.SpecId];
        }

        public static BuildingSpec GetSpec(this PlayerBuildingEntity o)
        {
            return GameLogic.Specs.Buildings[o.SpecID];
        }

        public static TileSpec GetSpec(this TileEntity o)
        {
            return GameLogic.Specs.Tiles[o.TileId];
        }

        public static DungeonSpec GetSpec(this DungeonEntity o)
        {
            return GameLogic.Specs.Dungeons[o.SpecID];
        }
    }

}

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
        public static BuildingSpec GetSpec(this PlayerBuildingEntity o)
        {
            return o.Game.Specs.Buildings[o.SpecId];
        }

        public static TileSpec GetSpec(this TileEntity o)
        {
            return o.Game.Specs.Tiles[o.SpecId];
        }

        public static DungeonSpec GetSpec(this DungeonEntity o)
        {
            return o.Game.Specs.Dungeons[o.SpecId]; 
        }
    }

}

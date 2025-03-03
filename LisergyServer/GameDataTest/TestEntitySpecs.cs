using Game.Engine;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Player;
using Game.Systems.Resources;
using GameData;
using GameData.Specs;
using System;

namespace GameDataTest
{
    public class TestEntitySpecs
    {
        public static void Generate(ref GameSpec spec)
        {
            spec.Entities[(int)EntityType.Party] = new EntitySpec()
            {
                Name = "Party",
                Components = Serialization.FromAnyTypes(new IComponent[] {
                    new MapPlaceableComponent(), new BattleGroupComponent(), new PartyComponent(), new EntityVisionComponent(),
                    new MovementComponent(), new MovespeedComponent(), new HarvesterComponent(),
                    new MovespeedComponent() {MoveDelay = TimeSpan.FromSeconds(0.3)},
                    new CargoComponent() { MaxWeight = 100 }
                }),
                Icon = new ArtSpec("Assets/Addressables/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
            };

            spec.Entities[(int)EntityType.Building] = new EntitySpec()
            {
                Name = "Building",
                Components = Serialization.FromAnyTypes(new IComponent[] {
                    new MapPlaceableComponent(), new BuildingComponent(), new PlayerBuildingComponent(),
                    new EntityVisionComponent(),
                }),
                Icon = new ArtSpec("Assets/Addressables/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
            };

            spec.Entities[(int)EntityType.Dungeon] = new EntitySpec()
            {
                Name = "Dungeon",
                Components = Serialization.FromAnyTypes(new IComponent[] {
                    new DungeonComponent(),
                    new BuildingComponent(),
                    new MapPlaceableComponent(),
                    new BattleGroupComponent()
                }),
                Icon = new ArtSpec("Assets/Addressables/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
            };

            spec.Entities[(int)EntityType.Player] = new EntitySpec()
            {
                Name = "Player",
                Components = Serialization.FromAnyTypes(new IComponent[] {
                    new PlayerDataComponent(),
                    new PlayerVisibilityComponent(),
                }),
            };

            spec.Entities[(int)EntityType.Tile] = new EntitySpec()
            {
                Name = "Tile",
                Components = Serialization.FromAnyTypes(new IComponent[] {

                }),
            };
        }
    }
}

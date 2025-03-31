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
                    new MovespeedComponent() {MoveDelay = TimeSpan.FromSeconds(1)},
                    new CargoComponent() { MaxWeight = 100 }
                }).ToArray(),
                Icon = new ArtSpec("res://Content/Art/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
            };

            spec.Entities[(int)EntityType.Building] = new EntitySpec()
            {
                Name = "Building",
                Components = Serialization.FromAnyTypes(new IComponent[] {
                    new MapPlaceableComponent(), new ConstructionComponent(), new PlayerBuildingComponent(),
                    new EntityVisionComponent(),
                }).ToArray(),
                Icon = new ArtSpec("res://Content/Art/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
            };

            spec.Entities[(int)EntityType.Dungeon] = new EntitySpec()
            {
                Name = "Dungeon",
                Components = Serialization.FromAnyTypes(new IComponent[] {
                    new DungeonComponent(),
                    new ConstructionComponent(),
                    new MapPlaceableComponent(),
                    new BattleGroupComponent()
                }).ToArray(),
                Icon = new ArtSpec("res://Content/Art/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
            };

            spec.Entities[(int)EntityType.Player] = new EntitySpec()
            {
                Name = "Player",
                Components = Serialization.FromAnyTypes(new IComponent[] {
                    new PlayerDataComponent(),
                    new PlayerVisibilityComponent(),
                    new CargoComponent()
                    {
                        Items = spec.StartingResources
                    },
                }).ToArray(),
            };

            spec.Entities[(int)EntityType.Tile] = new EntitySpec()
            {
                Name = "Tile",
                Components = Serialization.FromAnyTypes(new IComponent[] {

                }).ToArray(),
            };
        }
    }
}

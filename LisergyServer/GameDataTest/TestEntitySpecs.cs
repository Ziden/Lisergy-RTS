using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Party;
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
            spec.Entities[0] = new EntitySpec()
            {
                Name = "Party",
                Components = new IComponent [] { 
                    new MapPlacementComponent(), new BattleGroupComponent(), new PartyComponent(), new EntityVisionComponent(), 
                    new CourseComponent(), new MovespeedComponent(), new HarvesterComponent(),
                    new MovespeedComponent() {MoveDelay = TimeSpan.FromSeconds(0.3)},
                    new CargoComponent() { MaxWeight = 100 }
                },
                Icon = new ArtSpec("Assets/Addressables/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
                SpecId = 0,
            };

            spec.Entities[1] = new EntitySpec()
            {
                Name = "Building",
                Components = new IComponent[] {
                    new MapPlacementComponent(), new BuildingComponent(), new PlayerBuildingComponent(), 
                    new EntityVisionComponent(), new BuildingComponent(),
                },
                Icon = new ArtSpec("Assets/Addressables/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png"),
                SpecId = 1,
            };
        }
    }
}

using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
using Game.Tile;

namespace Game
{
    public class GameSystems
    {
        public static void SetupSystems()
        {
           AddSystem(new BuildingSystem());
           AddSystem(new BattleGroupSystem());
           AddSystem(new PlayerBuildingSystem());
           AddSystem(new DungeonSystem());
           AddSystem(new EntityVisionSystem());
           AddSystem(new EntityMovementSystem());
           AddSystem(new TileVisibilitySystem());
           AddSystem(new PartySystem());
           AddSystem(new TileHabitantsSystem());
        }

        private static void AddSystem<ComponentType, EntityType>(GameSystem<ComponentType, EntityType> system) where ComponentType : IComponent where EntityType : IEntity
        {
            TypedSystemRegistry<ComponentType, EntityType>.AddSystem(system);
        }
    }
}

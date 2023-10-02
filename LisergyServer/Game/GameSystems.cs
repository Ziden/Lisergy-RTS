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
    public static class GameSystems
    {
        static GameSystems() {
            if(!IsLoaded)
            SetupSystems();
        }

        public static bool IsLoaded { get; private set; }

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
           IsLoaded = true;
        }

        private static void AddSystem<ComponentType>(GameSystem<ComponentType> system) where ComponentType : IComponent 
        {
            ComponentSystemRegistry<ComponentType>.AddSystem(system);
        }
    }
}

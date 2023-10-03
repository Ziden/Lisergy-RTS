using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
using Game.Tile;
using System.Collections.Generic;
using System;
using Game.Events;

namespace Game
{
    public interface ISystems {
        BuildingSystem Building { get; }
        BattleGroupSystem BattleGroup { get; }
        PlayerBuildingSystem PlayerBuilding { get; }
        DungeonSystem Dungeons { get; }
        EntityVisionSystem EntityVision { get; }
        EntityMovementSystem EntityMovement { get; }
        TileVisibilitySystem TileVisibility { get; }
        PartySystem Party { get; }
        TileHabitantsSystem TileHabitants { get; }
        void CallEvent<EventType>(Type componentType, IEntity entity, EventType ev) where EventType : BaseEvent;
    }

    public class GameSystems : ISystems
    {
        private readonly Dictionary<Type, IGameSystem> _systems = new Dictionary<Type, IGameSystem>();

        public GameLogic Game { get; private set; }

        // Remove when logic is isolated in systems and no need to call events outside systems logic
        public static GameSystems HACK_REMOVE_ME;

        public GameSystems(GameLogic game) 
        {
            Game = game;
            AddSystem(Building = new BuildingSystem(game));
            AddSystem(BattleGroup = new BattleGroupSystem(game));
            AddSystem(PlayerBuilding = new PlayerBuildingSystem(game));
            AddSystem(Dungeons = new DungeonSystem(game));
            AddSystem(EntityVision = new EntityVisionSystem(game));
            AddSystem(EntityMovement = new EntityMovementSystem(game));
            AddSystem(TileVisibility = new TileVisibilitySystem(game));
            AddSystem(Party = new PartySystem(game));
            AddSystem(TileHabitants = new TileHabitantsSystem(game));

            HACK_REMOVE_ME = this;
        }

        public BuildingSystem Building { get; private set; }
        public BattleGroupSystem BattleGroup { get; private set; }
        public PlayerBuildingSystem PlayerBuilding { get; private set; }
        public DungeonSystem Dungeons { get; private set; }
        public EntityVisionSystem EntityVision { get; private set; }
        public EntityMovementSystem EntityMovement { get; private set; }
        public TileVisibilitySystem TileVisibility { get; private set; }
        public PartySystem Party { get; private set; }
        public TileHabitantsSystem TileHabitants { get; private set; }

        private void AddSystem<ComponentType>(GameSystem<ComponentType> system) where ComponentType : IComponent 
        {
            _systems[typeof(ComponentType)] = system;
            system.OnEnabled();
        }

        public void CallEvent<EventType>(Type componentType, IEntity entity, EventType ev) where EventType : BaseEvent
        {
            if (_systems.TryGetValue(componentType, out var system)) system.CallEvent(entity, ev);
        }
    }
}

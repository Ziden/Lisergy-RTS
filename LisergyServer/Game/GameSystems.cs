﻿using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
using System.Collections.Generic;
using System;
using Game.Events;
using Game.Systems.Map;
using Game.Systems.Player;
using Game.DataTypes;

namespace Game
{
    public interface ISystems {
        BuildingSystem Building { get; }
        BattleGroupSystem BattleGroup { get; }
        PlayerBuildingSystem PlayerBuilding { get; }
        DungeonSystem Dungeons { get; }
        EntityVisionSystem EntityVision { get; }
        CourseSystem EntityMovement { get; }
        TileVisibilitySystem TileVisibility { get; }
        MapSystem Map { get; }
        PartySystem Party { get; }
        TileHabitantsSystem TileHabitants { get; }
        PlayerSystem Players { get; }
        void CallEvent(IEntity entity, IBaseEvent ev);
    }

    public class GameSystems : ISystems
    {
        private readonly DefaultValueDictionary<Type, List<IGameSystem>> _systems = new DefaultValueDictionary<Type, List<IGameSystem>>();

        private LisergyGame _game;

        public GameSystems(LisergyGame game) 
        {
            _game = game;
            AddSystem(Building = new BuildingSystem(game));
            AddSystem(BattleGroup = new BattleGroupSystem(game));
            AddSystem(PlayerBuilding = new PlayerBuildingSystem(game));
            AddSystem(Dungeons = new DungeonSystem(game));
            AddSystem(EntityVision = new EntityVisionSystem(game));
            AddSystem(EntityMovement = new CourseSystem(game));
            AddSystem(TileVisibility = new TileVisibilitySystem(game));
            AddSystem(Party = new PartySystem(game));
            AddSystem(TileHabitants = new TileHabitantsSystem(game));
            AddSystem(Map = new MapSystem(game));
            AddSystem(Players = new PlayerSystem(game));
        }

        public MapSystem Map { get; private set; }
        public BuildingSystem Building { get; private set; }
        public BattleGroupSystem BattleGroup { get; private set; }
        public PlayerBuildingSystem PlayerBuilding { get; private set; }
        public DungeonSystem Dungeons { get; private set; }
        public EntityVisionSystem EntityVision { get; private set; }
        public CourseSystem EntityMovement { get; private set; }
        public TileVisibilitySystem TileVisibility { get; private set; }
        public PartySystem Party { get; private set; }
        public TileHabitantsSystem TileHabitants { get; private set; }
        public PlayerSystem Players { get; private set; }

        private void AddSystem<ComponentType>(GameSystem<ComponentType> system) where ComponentType : unmanaged, IComponent 
        {
            _systems[typeof(ComponentType)].Add(system);
            system.OnEnabled();
        }

        public void CallEvent(IEntity entity, IBaseEvent ev)
        {
            _game.Events.Call(ev);
            foreach (var componentType in entity.Components.All())
            {
                CallSystemEvent(componentType, entity, ev);
            }
        }

        private void CallSystemEvent<EventType>(Type componentType, IEntity entity, EventType ev) where EventType : IBaseEvent
        {
            if (_systems.TryGetValue(componentType, out var systems))
            {
                foreach(var system in systems) system.CallEvent(entity, ev);
            }
            
        }
    }
}
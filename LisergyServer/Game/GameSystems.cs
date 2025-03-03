using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Player;
using Game.Systems.Resources;
using Game.Systems.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game
{

    public class GameSystems
    {
        private readonly DefaultValueDictionary<Type, List<IGameSystem>> _systems = new DefaultValueDictionary<Type, List<IGameSystem>>();

        private LisergyGame _game;

        public GameSystems(LisergyGame game)
        {
            _game = game;
            AddSystem(Building = new BuildingSystem(game));
            AddSystem(BattleGroup = new BattleGroupSystem(game));
            AddSystem(Dungeons = new DungeonSystem(game));
            AddSystem(Deltacompression = new DeltaCompressionSystem(game));
            AddSystem(EntityVision = new EntityVisionSystem(game));
            AddSystem(EntityMovement = new MovementSystem(game));
            AddSystem(Party = new PartySystem(game));
            AddSystem(Tile = new TileSystem(game));
            AddSystem(Map = new MapSystem(game));
            AddSystem(Players = new PlayerSystem(game));
            AddSystem(Harvesting = new HarvestingSystem(game));
            AddSystem(Resources = new ResourceSystem(game));
            AddSystem(Cargo = new CargoSystem(game));
        }

        public DeltaCompressionSystem Deltacompression { get; private set; }
        public MapSystem Map { get; private set; }
        public BuildingSystem Building { get; private set; }
        public BattleGroupSystem BattleGroup { get; private set; }
        public DungeonSystem Dungeons { get; private set; }
        public EntityVisionSystem EntityVision { get; private set; }
        public MovementSystem EntityMovement { get; private set; }
        public PartySystem Party { get; private set; }
        public TileSystem Tile { get; private set; }
        public PlayerSystem Players { get; private set; }
        public HarvestingSystem Harvesting { get; private set; }
        public ResourceSystem Resources { get; private set; }
        public CargoSystem Cargo { get; private set; }

        private void AddSystem<ComponentType>(GameSystem<ComponentType> system) where ComponentType : IComponent
        {
            _systems[typeof(ComponentType)].Add(system);
            if (_game.IsClientGame)
            {
                var sync = system.GetType().GetCustomAttribute(typeof(SyncedSystem)) as SyncedSystem;
                if (sync == null) return;
            }
            _game.Log.Debug($"Registered System {system.GetType().Name}");
            system.RegisterListeners();
        }

        public void CallEvent(IEntity entity, IBaseEvent ev)
        {
            _game.Events.Call(ev);
            foreach (var componentType in entity.Components.AllTypes().ToArray())
            {
                CallSystemEvent(componentType, entity, ev);
            }
        }

        private void CallSystemEvent<EventType>(Type componentType, IEntity entity, EventType ev) where EventType : IBaseEvent
        {
            if (_systems.TryGetValue(componentType, out var systems))
            {
                foreach (var system in systems) system.CallEvent(entity, ev);
            }

        }
    }
}

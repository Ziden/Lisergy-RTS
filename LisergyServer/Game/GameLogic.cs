using Game.Engine.ECLS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.DeltaTracker;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Player;
using Game.Systems.Resources;
using Game.Systems.Tile;

namespace Game
{
    public class EntityLogic
    {
        public DungeonLogic Dungeon => _systems.Dungeons.GetLogic(_entity);
        public DeltaCompressionLogic DeltaCompression => _systems.Deltacompression.GetLogic(_entity);
        public MapLogic Map => _systems.Map.GetLogic(_entity);
        public BattleGroupLogic BattleGroup => _systems.BattleGroup.GetLogic(_entity);
        public BuildingLogic Building => _systems.Building.GetLogic(_entity);
        public PlayerLogic Player => _systems.Players.GetLogic(_entity);
        public MovementLogic Movement => _systems.EntityMovement.GetLogic(_entity);
        public HarvestingLogic Harvesting => _systems.Harvesting.GetLogic(_entity);
        public CargoLogic Cargo => _systems.Cargo.GetLogic(_entity);
        public EntityVisionLogic Vision => _systems.EntityVision.GetLogic(_entity);
        public TileLogic Tile => _systems.Tile.GetLogic(_entity);

        private GameSystems _systems;
        private IEntity _entity;

        public EntityLogic(GameSystems systems) { _systems = systems; }
        public EntityLogic GetLogic(IEntity entity)
        {
            _entity = entity;
            return this;
        }
    }

    public interface IGameLogic
    {
        public GameSystems Systems { get; }
        public EntityLogic GetEntityLogic(IEntity e);
    }

    public class GameLogic : IGameLogic
    {
        public GameSystems Systems { get; set; }
        private EntityLogic _entityLogic;

        public GameLogic(LisergyGame game)
        {
            Systems = new GameSystems(game);
            _entityLogic = new EntityLogic(Systems);
        }

        /// <summary>
        /// Gets a reusable logic object for the given entity
        /// </summary>
        public EntityLogic GetEntityLogic(IEntity e) => _entityLogic.GetLogic(e);
    }
}

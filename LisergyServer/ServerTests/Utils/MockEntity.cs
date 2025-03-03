using Game;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;

namespace ServerTests.Utils
{
    /// <summary>
    /// A mock entity that bypasses the automatic component initialization performed by GameEntities.SetupArchetype
    /// This allows for clean component testing without pre-filled components from archetypes
    /// </summary>
    public class MockEntity : IEntity
    {
        private GameId _entityId;
        private IGame _game;
        
        public ComponentSet Components { get; private set; }
        public EntityType EntityType { get; }
        public ref readonly GameId EntityId => ref _entityId;
        public IGame Game => _game;
        public GameId OwnerID { get; set; } = GameId.ZERO;
        public EntityLogic Logic => Game.Logic.GetEntityLogic(this);

        public MockEntity(GameId id, IGame game, EntityType type)
        {
            _game = game;
            _entityId = id;
            EntityType = type;
            Components = new ComponentSet(this);
        }

        public T Get<T>() where T : IComponent => Components.Get<T>();
        
        public void Save<T>(in T component) where T : IComponent => Components.Save(component);
        
        public override string ToString()
        {
            return $"<MockEntity {EntityType} {EntityId} {Components}>";
        }
    }
}

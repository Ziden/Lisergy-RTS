using Game.Engine.DataTypes;
using Game.Entities;

namespace Game.Engine.ECLS
{
    /// <summary>
    /// Represents an Entity in the game.
    /// An entity is something that has a set of components which dictates its behaviour, an owner and
    /// logic that can be used to modify the components states.
    /// </summary>
    public interface IEntity
    {
        public EntityType EntityType { get; }
        public ComponentSet Components { get; }
        public T Get<T>() where T : IComponent;
        public void Save<T>(in T c) where T : IComponent;
        public ref readonly GameId EntityId { get; }
        public IGame Game { get; }
        public GameId OwnerID { get; }
        public EntityLogic Logic { get; }
    }
}

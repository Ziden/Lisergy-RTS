using Game.DataTypes;
using Game.Network;

namespace Game.ECS
{
    /// <summary>
    /// Represents an Entity in the game.
    /// An entity is something that has a set of components which dictates its behaviour, an owner and
    /// logic that can be used to modify the components states.
    /// </summary>
    public interface IEntity : IEntityDeltaTrackable
    {
        public EntityType EntityType { get; }
        public IComponentSet Components { get; }
        public ref T Get<T>() where T : unmanaged, IComponent;
        public void Save<T>(in T c) where T : unmanaged, IComponent;
        public ref readonly GameId EntityId { get; }
        public ref readonly GameId OwnerID { get; }
        public IGame Game { get; }
        public IEntityLogic EntityLogic { get; }
    }
}

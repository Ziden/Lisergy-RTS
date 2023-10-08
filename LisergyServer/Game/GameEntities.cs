using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Systems.Player;
using System.Collections.Generic;

namespace Game
{
    public enum EntityType : byte
    {
        Player, Party, Dungeon, Building, Tile
    }

    public interface IGameEntities
    {
        T CreateEntity<T>(PlayerEntity owner) where T : IEntity;

        IEntity this[in GameId id] {get;}

        IDeltaCompression DeltaCompression { get; }
    }

    public class GameEntities : IGameEntities
    {
        private IGame _game;
        private readonly Dictionary<GameId, IEntity> _entities = new Dictionary<GameId, IEntity>();
        private DeltaCompression _deltaTracker = new DeltaCompression();

        public IDeltaCompression DeltaCompression  => _deltaTracker;

        public GameEntities(IGame game)
        {
            _game = game;
        }

        public T CreateEntity<T>(PlayerEntity owner) where T : IEntity
        {
            var instance = CreateEntityInstance<T>(owner);
            if(instance != null) _entities[instance.EntityId] = instance;
            return (T)instance;
        }

        public IEntity CreateEntityInstance<T>(PlayerEntity owner) where T : IEntity
        {
            if (typeof(T) == typeof(DungeonEntity)) return new DungeonEntity(_game);
            if (typeof(T) == typeof(PartyEntity)) return new PartyEntity(_game, owner);
            if (typeof(T) == typeof(PlayerBuildingEntity)) return new PlayerBuildingEntity(_game, owner);
            return null;
        }

        public IEntity this[in GameId id] => _entities[id];
    }
}

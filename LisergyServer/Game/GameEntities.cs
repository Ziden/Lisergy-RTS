using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Game
{
    public enum EntityType : byte
    {
        Player, Party, Dungeon, Building, Tile
    }

    /// <summary>
    /// Represents the entities that are currently in the game
    /// </summary>
    public interface IGameEntities
    {
        /// <summary>
        /// Creates a new entity based on a type
        /// </summary>
        IEntity CreateEntity(in GameId owner, in EntityType type);

        /// <summary>
        /// Gets an entity by its given ID. Can return null if entity do not exists.
        /// </summary>
        IEntity this[in GameId id] {get;}

        /// <summary>
        /// Holds all modified entitie deltas for the current input cycle
        /// </summary>
        IDeltaCompression DeltaCompression { get; }
    }

    public class GameEntities : IGameEntities, IDisposable
    {
        private IGame _game;
        internal readonly Dictionary<GameId, IEntity> _entities = new Dictionary<GameId, IEntity>();
        internal DeltaCompression _deltaTracker = new DeltaCompression();

        public IDeltaCompression DeltaCompression  => _deltaTracker;

        public GameEntities(IGame game)
        {
            _game = game;
        }

        public IEntity CreateEntity(in GameId owner, in EntityType type)
        {
            IEntity e = null;
            if (type == EntityType.Dungeon) e = new DungeonEntity(_game);
            else if (type == EntityType.Party) e = new PartyEntity(_game, owner);
            else if (type == EntityType.Building) e = new PlayerBuildingEntity(_game, owner);
            else throw new Exception($"Entity type {type} is not createable");
            _entities[e.EntityId] = e;
            if(!owner.IsZero())
            {
                var player = _game.World.Players[owner];
                player.AddOwnedEntity(e);
            }
            e.DeltaFlags.SetFlag(DeltaFlag.CREATED);
            return e;
        }

        public void Dispose()
        {
            foreach (var e in _entities.Values) e.Components.Dispose();
        }

        public IEntity this[in GameId id] {
            get
            {
                _entities.TryGetValue(id, out var entity);
                return entity;
            }
        }

        public IReadOnlyCollection<IEntity> AllEntities => _entities.Values;
    }
}

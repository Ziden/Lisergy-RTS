using System;
using System.Collections.Generic;
using System.Linq;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.DeltaTracker;

namespace Game.Entities
{
	public enum EntityType : byte
	{
		Player,
		Party,
		Dungeon,
		Building,
		Tile
	}

	public class GameEntities : IDisposable
	{
		private readonly Dictionary<GameId, NodeTree<IEntity>> _entities = new Dictionary<GameId, NodeTree<IEntity>>();
		private readonly IGame _game;

		public GameEntities(IGame game)
		{
			_game = game;
		}

		public IEntity this[in GameId id]
		{
			get
			{
				_entities.TryGetValue(id, out var entity);
				return entity?.Data;
			}
		}

		public IReadOnlyCollection<IEntity> AllEntities => _entities.Values.Select(e => e.Data).ToArray();

		public void Dispose()
		{
		}

		private void SetupArchetype(IEntity entity, in EntityType id)
		{
			entity.Components.Save(new NetworkedComponent()); // TODO: think better
			var spec = _game.Specs.Entities[(int) id];
			var components = Serialization.ToAnyTypes<object>(spec.Components);
			foreach (var component in components)
			{
				entity.Components.GetComponents()[component.GetType()] = (IComponent) component;
				entity.Components.OnAfterAdded(component.GetType());
			}
		}

        /// <summary>
        ///     TODO: Remove this. This is needed because client predicts creating tiles with wrong ids.
        /// </summary>
        public void UpdateEntityId(BaseEntity e, GameId newId)
		{
			var oldId = e.EntityId;
			e._entityId = newId;
			var oldNode = _entities[oldId];
			_entities.Remove(oldId);
			_entities.Add(newId, oldNode);
		}

		public IEntity CreateEntity(in EntityType type, GameId parent = default, GameId entityId = default)
		{
			if (entityId == GameId.ZERO) entityId = GameId.Generate();
			if (parent == entityId) parent = GameId.ZERO;

			var e = new BaseEntity(entityId, _game, type);
			SetupArchetype(e, type);
			var entityNode = new NodeTree<IEntity>(e);
			_entities[e.EntityId] = entityNode;

			if (parent != GameId.ZERO)
			{
				if (!_entities.TryGetValue(parent, out var parentNode))
					throw new Exception("Parent entity " + parent + " not found");
				parentNode.AddChild(entityNode);
			}

			e.Logic.DeltaCompression.Clear();
			e.Logic.DeltaCompression.SetFlag(DeltaFlag.CREATED);
			return e;
		}

		public IReadOnlyList<IEntity> GetChildren(in GameId owner)
		{
			var ret = new List<IEntity>();
			_entities[owner].Traverse(e =>
			{
				ret.Add(e);
				return true;
			});
			return ret;
		}

		public IReadOnlyList<IEntity> GetChildren(in GameId owner, EntityType type)
		{
			var ret = new List<IEntity>();
			_entities[owner].Traverse(e =>
			{
				if (e.EntityType == type) ret.Add(e);
				return true;
			});
			return ret;
		}

		public IEntity GetParent(GameId entityId)
		{
			return _entities[entityId]?.Parent?.Data;
		}

		public bool IsParent(in GameId owner, in GameId o)
		{
			var parent = GetParent(o);
			return parent?.EntityId == owner;
		}

		public void SetParent(in GameId owner, in GameId owned)
		{
			_entities[owner].AddChild(_entities[owned]);
		}
	}
}
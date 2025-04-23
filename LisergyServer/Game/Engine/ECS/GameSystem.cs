using System;
using Game.Engine.Events;
using Game.Engine.Events.Bus;
using Game.World;

namespace Game.Engine.ECLS
{
	public interface IGameSystem
	{
		void OnEntityEvent<EventType>(Action<IEntity, EventType> cb) where EventType : IBaseEvent;
		void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : IBaseEvent;
	}

	public abstract class GameSystem<ComponentType> : IGameSystem, IEventListener where ComponentType : IComponent
	{
		protected ComponentEventBus<ComponentType> EntityEvents = new ComponentEventBus<ComponentType>();

		public GameSystem(LisergyGame game)
		{
			Game = game;
		}

		public IGame Game { get; }
		public IGameLogic GameLogic => Game.Logic;
		public IGameWorld World => Game.World;
		public IGamePlayers Players => World.Players;

        /// <summary>
        ///     Fired whenever an entity receives an event that matches the component defined in the system component type
        /// </summary>
        public void OnEntityEvent<EventType>(Action<IEntity, EventType> cb) where EventType : IBaseEvent
		{
			EntityEvents.On(cb);
		}

		public void CallEvent<EventType>(IEntity entityType, EventType ev) where EventType : IBaseEvent
		{
			EntityEvents.CallEntityEvent(entityType, ev);
		}

		internal virtual void OnComponentAdded(IEntity owner, ComponentType component)
		{
		}

		public virtual void OnDisabled()
		{
		}

		public virtual void RegisterListeners()
		{
		}

		internal virtual void OnComponentRemoved(IEntity owner, ComponentType component)
		{
		}

		// TODO: Separate better entity and global events
        /// <summary>
        ///     Fired whenever any entity receive an event of the given type
        /// </summary>
        public void OnAnyEvent<EventType>(Action<EventType> cb) where EventType : IBaseEvent
		{
			Game.Events.On(this, cb);
		}

		public EntityLogic GetLogic(IEntity e)
		{
			return GameLogic.GetEntityLogic(e);
		}
	}

	public class LogicSystem<ComponentType, LogicType> : GameSystem<ComponentType> where ComponentType : IComponent
		where LogicType : BaseEntityLogic<ComponentType>
	{
		private readonly LogicType _logic;

		public LogicSystem(LisergyGame game) : base(game)
		{
			_logic = Activator.CreateInstance<LogicType>();
		}

        /// <summary>
        ///     Gets given logic for given entity
        /// </summary>
        public new LogicType GetLogic(IEntity entity)
		{
			_logic.CurrentEntity = entity;
			return _logic;
		}
	}
}
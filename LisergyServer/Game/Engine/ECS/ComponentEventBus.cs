using System;
using System.Runtime.CompilerServices;
using Game.Engine.Events;
using Game.Engine.Events.Bus;

[assembly: InternalsVisibleTo("Tests")]

namespace Game.Engine.ECLS
{
	public class ComponentEventBus<ComponentType> : IEventListener
	{
		internal EventBus<IBaseEvent> _bus = new EventBus<IBaseEvent>();
		private IEntity _currentEntity;

		public void On<EventType>(Action<IEntity, EventType> callback) where EventType : IBaseEvent
		{
			void ComponentEventWrapper(EventType ev)
			{
				callback(_currentEntity, ev);
			}

			_bus.On<EventType>(this, ComponentEventWrapper);
		}

		public void On<EventType>(Action<EventType> callback) where EventType : IBaseEvent
		{
			_bus.On(this, callback);
		}

		public void CallEntityEvent<EventType>(IEntity entity, EventType ev) where EventType : IBaseEvent
		{
			_currentEntity = entity;
			_bus.Call(ev);
		}
	}
}
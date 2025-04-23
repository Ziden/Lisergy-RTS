using System;
using System.Linq;
using Game.Engine.DataTypes;
using Game.Entities;

namespace Game.Engine.ECLS
{
	[Serializable]
	public class SerializedEntity
	{
		public object[] Components;
		public GameId EntityId;
		public EntityType EntityType;
		public GameId OwnerId;

		public SerializedEntity(IEntity entity)
		{
			EntityType = entity.EntityType;
			EntityId = entity.EntityId;
			OwnerId = entity.OwnerID;
			Components = entity.Components.AllComponents().ToArray();
		}
	}
}
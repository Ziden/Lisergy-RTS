using System;
using System.Linq;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Network;
using Game.Entities;

namespace Game.Events.ServerEvents
{
	[Serializable]
	public class EntityUpdatePacket : BasePacket, IServerPacket
	{
		public GameId EntityId;
		public GameId OwnerId;
		public uint[] RemovedComponentIds;
		public object[] SyncedComponents;
		public EntityType Type;

		public EntityUpdatePacket()
		{
		}

		public EntityUpdatePacket(IEntity entity)
		{
			Type = entity.EntityType;
			EntityId = entity.EntityId;
			OwnerId = entity.OwnerID;
		}

		public T GetComponent<T>()
		{
			return (T) SyncedComponents.FirstOrDefault(c => c.GetType() == typeof(T));
		}

		public override string ToString()
		{
			return $"<EntityUpdate {Type} {EntityId} Components={string.Join(',', SyncedComponents.ToList())}>";
		}
	}
}
using System;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Network;

namespace Game.Network.ServerPackets
{
	[Serializable]
	public class EntityDestroyPacket : BasePacket, IServerPacket
	{
		public GameId EntityID;

		public GameId OwnerID;

		public EntityDestroyPacket()
		{
		}

		public EntityDestroyPacket(IEntity entity)
		{
			OwnerID = entity.OwnerID;
			EntityID = entity.EntityId;
		}
	}
}
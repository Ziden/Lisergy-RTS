using System;
using Game.Engine.Network;

namespace Game.Network.ServerPackets
{
	[Serializable]
	public class InvalidSessionPacket : BasePacket, IServerPacket
	{
	}
}
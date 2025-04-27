using System;
using System.Collections.Generic;
using Game.Engine.DataTypes;
using Game.Engine.Network;

namespace Game.Network.ClientPackets
{
	[Serializable]
	public class ChatPacket : BasePacket, IClientPacket
	{
		public string Message;
		public string Name;
		public GameId Owner;
		public DateTime Time;
	}

	[Serializable]
	public class ChatLogPacket : BasePacket, IServerPacket
	{
		public List<ChatPacket> Messages;
	}
}
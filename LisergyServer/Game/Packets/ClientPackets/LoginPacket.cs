using System;
using Game.Engine.Network;

namespace Game.Network.ClientPackets
{
	[Serializable]
	public class LoginPacket : BasePacket, IClientPacket
	{
		public string Login;
		public string Password;
		public int SpecVersion;
	}
}
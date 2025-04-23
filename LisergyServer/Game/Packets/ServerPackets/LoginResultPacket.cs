using System;
using Game.Engine.Network;
using Game.Systems.Player;

namespace Game.Events.ServerEvents
{
	[Serializable]
	public class LoginResultPacket : BasePacket, IServerPacket
	{
		public PlayerProfileComponent Profile;
		public bool Success;
		public string Token;
		public TimeSpan TokenDuration;
	}
}
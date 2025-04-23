using System;
using Game.Engine.Network;
using Game.Systems.Battle.BattleActions;

namespace Game.Network.ServerPackets
{
	[Serializable]
	public class BattleActionResultPacket : BasePacket, IServerPacket
	{
		public ActionResult ActionResult;
	}
}
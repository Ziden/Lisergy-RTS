using System;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Network;
using Game.Systems.Battle.Data;
using Game.Systems.Map;
using Game.World;

namespace Game.Network.ServerPackets
{
    /// <summary>
    ///     Packet sent to other services whenever a battle started so its picked up and processed
    /// </summary>
    [Serializable]
	public class BattleQueuedPacket : BasePacket, IServerPacket
	{
		public BattleGroupData Attacker;
		public GameId BattleID;
		public BattleGroupData Defender;
		public Location Position;

		public BattleQueuedPacket(GameId battleId, IEntity attacker, IEntity defender)
		{
			var pos = attacker.Get<MapPlacementComponent>();
			BattleID = battleId;
			Attacker = new BattleGroupData(attacker);
			Defender = new BattleGroupData(defender);
			Position = pos.Position;
		}
	}
}
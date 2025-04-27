using System;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Systems.Battle.Data;
using Game.World;

namespace Game.Network.ServerPackets
{
    /// <summary>
    ///     Packet sent to player whenever a battle started
    /// </summary>
    [Serializable]
	public class BattleStartPacket : BasePacket, IServerPacket
	{
		public BattleGroupData Attacker;
		public GameId BattleID;
		public BattleGroupData Defender;
		public Location Position;

		public BattleStartPacket()
		{
		}

		public BattleStartPacket(in GameId battleId, in Location position, in BattleGroupData attacker,
			in BattleGroupData defender)
		{
			BattleID = battleId;
			Attacker = attacker;
			Defender = defender;
			Position = position;
		}
	}
}
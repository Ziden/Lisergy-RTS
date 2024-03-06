using Game.Engine.DataTypes;
using Game.Engine.ECS;
using Game.Engine.Network;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.World;
using System;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Packet sent to other services whenever a battle started so its picked up and processed
    /// </summary>
    [Serializable]
    public class BattleQueuedPacket : BasePacket, IServerPacket
    {
        public Location Position;
        public GameId BattleID;
        public BattleTeamData Attacker;
        public BattleTeamData Defender;

        public BattleQueuedPacket(GameId battleId, IEntity attacker, IEntity defender)
        {
            var pos = attacker.Get<MapPlacementComponent>();
            BattleID = battleId;
            Attacker = new BattleTeamData(attacker);
            Defender = new BattleTeamData(defender);
            Position = pos.Position;
        }
    }
}

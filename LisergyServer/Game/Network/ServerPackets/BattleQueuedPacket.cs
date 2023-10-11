using Game.Battle.Data;
using Game.DataTypes;
using Game.ECS;
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
        public Position Position;
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

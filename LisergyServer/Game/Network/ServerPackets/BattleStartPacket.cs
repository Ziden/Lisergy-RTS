using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.World;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class BattleStartPacket : ServerPacket
    {
        public Position Position;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public BattleStartPacket(GameId battleId, IEntity attacker, IEntity defender)
        {
            var pos = attacker.Get<MapPlacementComponent>();
            BattleID = battleId;
            Attacker = new BattleTeam(attacker, attacker.Get<BattleGroupComponent>());
            Defender = new BattleTeam(defender, defender.Get<BattleGroupComponent>());
            Position = pos.Position;
        }
    }
}

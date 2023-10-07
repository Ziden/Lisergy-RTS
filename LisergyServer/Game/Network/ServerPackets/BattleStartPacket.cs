using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Map;
using System;

namespace Game.Network.ServerPackets
{
    [Serializable]
    public class BattleStartPacket : ServerPacket
    {
        public ushort X;
        public ushort Y;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public BattleStartPacket(GameId battleId, IEntity attacker, IEntity defender)
        {
            var pos = attacker.Get<MapPositionComponent>();
            BattleID = battleId;
            Attacker = new BattleTeam(attacker, attacker.Get<BattleGroupComponent>());
            Defender = new BattleTeam(defender, defender.Get<BattleGroupComponent>());
            X = pos.X;
            Y = pos.Y;
        }

        public BattleStartPacket(GameId battleId, ushort x, ushort y, BattleTeam atk, BattleTeam def)
        {
            BattleID = battleId;
            Attacker = atk;
            Defender = def;
            X = x;
            Y = y;
        }
    }
}

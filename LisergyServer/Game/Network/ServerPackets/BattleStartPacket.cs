using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
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
            var tile = (attacker as BaseEntity).Tile;
            BattleID = battleId;
            Attacker = new BattleTeam(attacker, attacker.Get<BattleGroupComponent>());
            Defender = new BattleTeam(defender, defender.Get<BattleGroupComponent>());
            X = tile.X;
            Y = tile.Y;
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

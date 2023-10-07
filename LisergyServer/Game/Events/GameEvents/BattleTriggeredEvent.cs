using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.Map;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// When a battle is triggered
    /// </summary>
    public class BattleTriggeredEvent : GameEvent
    {
        public ushort X;
        public ushort Y;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public BattleTriggeredEvent(GameId battleId, IEntity attacker, IEntity defender)
        {
            var pos = attacker.Get<MapPositionComponent>();
            BattleID = battleId;
            Attacker = new BattleTeam(attacker, attacker.Get<BattleGroupComponent>());
            Defender = new BattleTeam(defender, defender.Get<BattleGroupComponent>());
            X = pos.X;
            Y = pos.Y;
        }

        public BattleTriggeredEvent(GameId battleId, ushort x, ushort y, BattleTeam atk, BattleTeam def)
        {
            BattleID = battleId;
            Attacker = atk;
            Defender = def;
            X = x;
            Y = y;
        }
    }
}

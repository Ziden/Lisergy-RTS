using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.World;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// When a battle is triggered
    /// </summary>
    public class BattleTriggeredEvent : GameEvent
    {
        public Position Position;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public BattleTriggeredEvent(GameId battleId, IEntity attacker, IEntity defender)
        {
            var pos = attacker.Get<MapPlacementComponent>();
            BattleID = battleId;
            Attacker = new BattleTeam(attacker, attacker.Get<BattleGroupComponent>());
            Defender = new BattleTeam(defender, defender.Get<BattleGroupComponent>());
            Position = pos.Position;
        }
    }
}

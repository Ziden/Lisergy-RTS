using Game.Battle.Data;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.World;

namespace Game.Network.ServerPackets
{
    /// <summary>
    /// Triggered when a battle is triggered in game logic
    /// </summary>
    public class BattleTriggeredEvent : GameEvent
    {
        public Position Position;
        public GameId BattleID;
        public BattleTeamData Attacker;
        public BattleTeamData Defender;

        public BattleTriggeredEvent(GameId battleId, IEntity attacker, IEntity defender)
        {
            var pos = attacker.Get<MapPlacementComponent>();
            BattleID = battleId;
            Attacker = new BattleTeamData(attacker);
            Defender = new BattleTeamData(defender);
            Position = pos.Position;
        }
    }
}

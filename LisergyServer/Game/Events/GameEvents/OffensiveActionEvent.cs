using Game.ECS;
using Game.Systems.Battler;

namespace Game.Events.GameEvents
{
    public class OffensiveActionEvent : IGameEvent
    {
        public IEntity Attacker;
        public BattleGroupComponent AttackerGroup;

        public IEntity Defender;
        public BattleGroupComponent DefenderGroup;
    }
}

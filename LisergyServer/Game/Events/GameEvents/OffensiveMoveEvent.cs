using Game.ECS;

namespace Game.Events.GameEvents
{
    public class OffensiveMoveEvent : GameEvent
    {
        public IEntity Attacker;
        public IEntity Defender;
    }
}

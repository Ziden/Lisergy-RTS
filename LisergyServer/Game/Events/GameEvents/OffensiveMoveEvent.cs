namespace Game.Events.GameEvents
{
    public class OffensiveMoveEvent : GameEvent
    {
        public WorldEntity Attacker;
        public WorldEntity Defender;
    }
}

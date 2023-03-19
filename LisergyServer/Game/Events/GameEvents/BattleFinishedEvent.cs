using Game.Battle;
using Game.Battles;

namespace Game.Events.GameEvents
{
    public class BattleFinishedEvent : GameEvent
    {
        public TurnBattle Battle;
        public BattleHeader Header;
        public BattleTurnEvent[] Turns;

        public BattleFinishedEvent(TurnBattle battle, BattleHeader header, BattleTurnEvent[] turns)
        {
            Battle = battle;
            Header = header;
            Turns = turns;
        }
    }
}

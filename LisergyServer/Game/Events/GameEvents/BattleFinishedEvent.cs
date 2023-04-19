using Game.Battle;
using Game.Network.ServerPackets;

namespace Game.Events.GameEvents
{
    public class BattleFinishedEvent : GameEvent
    {
        public TurnBattle Battle;
        public CompleteBattleHeader Header;
        public BattleTurnLog[] Turns;

        public BattleFinishedEvent(TurnBattle battle, CompleteBattleHeader header, BattleTurnLog[] turns)
        {
            Battle = battle;
            Header = header;
            Turns = turns;
        }
    }
}

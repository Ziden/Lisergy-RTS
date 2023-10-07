using Game.Battle;
using Game.DataTypes;
using Game.Network.ServerPackets;

namespace Game.Events.GameEvents
{
    public class BattleFinishedEvent : GameEvent
    {
        public GameId Battle;
        public BattleHeader Header;
        public BattleTurnLog[] Turns;

        public BattleFinishedEvent(BattleHeader header, BattleTurnLog[] turns)
        {
            Battle = header.BattleID;
            Header = header;
            Turns = turns;
        }
    }
}

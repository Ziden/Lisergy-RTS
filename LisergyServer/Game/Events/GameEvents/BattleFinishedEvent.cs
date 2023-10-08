using Game.Battle.Data;
using Game.DataTypes;
using Game.Network.ServerPackets;

namespace Game.Events.GameEvents
{
    public class BattleFinishedEvent : GameEvent
    {
        public GameId Battle;
        public BattleHeaderData Header;
        public BattleTurnLog[] Turns;

        public BattleFinishedEvent(BattleHeaderData header, BattleTurnLog[] turns)
        {
            Battle = header.BattleID;
            Header = header;
            Turns = turns;
        }
    }
}

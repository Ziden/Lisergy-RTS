using Game.Battle.Data;
using Game.DataTypes;
using Game.Network.ServerPackets;

namespace Game.Events.GameEvents
{
    public class BattleFinishedEvent : IGameEvent
    {
        public GameId Battle;
        public BattleState Header;
        public BattleTurnLog[] Turns;
    }
}

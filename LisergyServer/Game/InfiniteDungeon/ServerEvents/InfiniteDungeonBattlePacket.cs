using Game.Battles;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class InfiniteDungeonBattlePacket: ServerEvent
    {
        public int Level;
        public BattleStartPacket BattleStartPacket;
    }
}

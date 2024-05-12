using Game.Systems.Battle.BattleActions;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Battle
{
    public class TurnLog
    {
        public byte RoundNumber;
        public List<BattleEvent> Events = new List<BattleEvent>();

        public TurnLog(byte round)
        {
            RoundNumber = round;
        }

        public override string ToString()
        {
            return string.Join(",", Events.Select(a => a.ToString()).ToArray());
        }
    }
}

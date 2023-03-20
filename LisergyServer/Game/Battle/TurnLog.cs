using Game.BattleActions;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battle
{
    public class TurnLog
    {
        public byte RoundNumber;
        public List<BattleAction> Actions = new List<BattleAction>();

        public TurnLog(byte round)
        {
            RoundNumber = round;
        }

        public override string ToString()
        {
            return string.Join(",", Actions.Select(a => a.ToString()).ToArray());
        }
    }
}

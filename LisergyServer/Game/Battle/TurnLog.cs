using System.Collections.Generic;
using System.Linq;

namespace Game.Battles.Actions
{
    public class TurnLog
    {
        public byte RoundNumber;
        public List<BattleAction> Actions = new List<BattleAction>();

        public TurnLog(byte round)
        {
            this.RoundNumber = round;
        }

        public override string ToString()
        {
            return string.Join(",", Actions.Select(a => a.ToString()).ToArray());
        }
    }
}

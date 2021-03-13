using System.Collections.Generic;
using System.Linq;

namespace Game.Battle.Log
{
    public class TurnLog
    {
        public byte RoundNumber;
        public List<ActionLog> Actions = new List<ActionLog>();

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

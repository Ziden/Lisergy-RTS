using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{
    [Serializable]
    public class Party
    {
        public List<Unit> Units = new List<Unit>();
        public string PartyID;
        public string BattleID;
    }
}

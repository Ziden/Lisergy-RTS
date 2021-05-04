using Game.Battles;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battle
{
    public class BattleJournal
    {
        private static Dictionary<string, BattleResultEvent> AllJournals = new Dictionary<string, BattleResultEvent>();

        private List<BattleResultEvent> Results = new List<BattleResultEvent>();
    }

    [Serializable]
    public class BattleJournalHeader
    {
        public bool AttackerWins;
        public string BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public DateTime Date;
    }


}

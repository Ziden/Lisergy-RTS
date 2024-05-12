using Game.Systems.Battle.BattleActions;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Battle
{
    /// <summary>
    /// Represents a record of what happened in a given battle.
    /// Will be filled while the battle runs.
    /// Will hold all input events of a given battle, that means a given battle
    /// can be replayed deterministically by the same given events.
    /// </summary>
    public class TurnBattleRecord
    {
        public List<TurnLog> Turns = new List<TurnLog>();

        public BattleTeam Attacker;
        public BattleTeam Defender;
        public BattleTeam Winner;

        public void NextTurn()
        {
            Turns.Add(new TurnLog((byte)(Turns.Count + 1)));
        }

        public void RecordEvent(BattleEvent action)
        {
            CurrentTurn.Events.Add(action);
        }

        public TurnLog CurrentTurn => Turns.Last();

        public override string ToString()
        {
            return $"<Battle {Attacker}vs{Defender} Rounds={Turns.Count} Winner={Winner}>";
        }
    }
}

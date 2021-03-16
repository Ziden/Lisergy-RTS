using Game.Battles.Log;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battles
{
    public class BattleResult
    {
        public List<TurnLog> Turns = new List<TurnLog>();

        public BattleTeam Attacker;
        public BattleTeam Defender;
        public BattleTeam Winner;

        public void NextTurn()
        {
            Turns.Add(new TurnLog((byte)(Turns.Count+1)));
        }

        public void AddAction(ActionLog action)
        {
            CurrentTurn.Actions.Add(action);
        }

        public ActionLog LastAction => CurrentTurn.Actions.First();

        public TurnLog CurrentTurn { get => Turns.Last(); }

        public override string ToString()
        {
            return $"<Battle {Attacker}vs{Defender} Rounds={Turns.Count} Winner={Winner}>";
        }
    }
}

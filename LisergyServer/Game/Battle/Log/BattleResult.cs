using Game.Battle.Log;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battle
{
    public class BattleResult
    {
        public List<TurnLog> Rounds = new List<TurnLog>();

        public BattleTeam Attacker;
        public BattleTeam Defender;
        public BattleTeam Winner;

        public void NextTurn()
        {
            Rounds.Add(new TurnLog((byte)(Rounds.Count+1)));
        }

        public void AddAction(ActionLog action)
        {
            CurrentTurn.Actions.Add(action);
        }

        public ActionLog LastAction => CurrentTurn.Actions.First();

        public TurnLog CurrentTurn { get => Rounds.Last(); }

        public override string ToString()
        {
            return $"<Battle {Attacker}vs{Defender} Rounds={Rounds.Count} Winner={Winner}>";
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Game.Systems.Battle.BattleActions;

namespace Game.Systems.Battle
{
	public class TurnLog
	{
		public List<BattleEvent> Events = new List<BattleEvent>();
		public byte RoundNumber;

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
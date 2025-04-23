using System;
using Game.Engine.Network;
using Game.Systems.Battle.BattleActions;

namespace Game.Systems.Battle
{
	[Serializable]
	public class BattleActionCommand : BasePacket, IGameCommand
	{
		public BattleAction Action;
		public string BattleID;

		public BattleActionCommand(string BattleID, BattleAction action)
		{
			Action = action;
			this.BattleID = BattleID;
		}

		public void Execute(IGame game)
		{
			throw new NotImplementedException();
		}
	}
}
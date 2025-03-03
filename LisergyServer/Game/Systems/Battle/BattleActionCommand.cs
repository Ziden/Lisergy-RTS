using Game.Engine.Network;
using Game.Systems.Battle.BattleActions;
using System;

namespace Game.Systems.Battle
{
    [Serializable]
    public class BattleActionCommand : BasePacket, IGameCommand
    {
        public BattleActionCommand(string BattleID, BattleAction action)
        {
            Action = action;
            this.BattleID = BattleID;
        }

        public BattleAction Action;
        public string BattleID;

        public void Execute(IGame game)
        {
            throw new NotImplementedException();
        }
    }
}

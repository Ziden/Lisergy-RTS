using Game.Battles;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battle
{
    public interface IBattleable
    {
        BattleTeam GetBattleTeam();

        void OnBattleComplete(string battleID);
    }
}

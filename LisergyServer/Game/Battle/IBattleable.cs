using Game.Battles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battle
{
    public interface IBattleable
    {
        BattleTeam GetBattleTeam();
    }
}

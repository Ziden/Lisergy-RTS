using Game.Battles;
using Game.Entity;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battle
{
    public interface IBattleable : IOwnable, IMapEntity, IUpdateable
    {
        BattleTeam GetBattleTeam();

        void OnBattleStarted(TurnBattle battle);

        void OnBattleFinished(TurnBattle battle, BattleHeader BattleHeader, BattleTurnEvent[] Turns);

        bool IsBattling { get; }

        bool IsDestroyed { get; }
    }
}

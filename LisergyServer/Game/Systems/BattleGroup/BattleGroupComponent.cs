using Game.DataTypes;
using Game.ECS;
using System;
using System.Collections.Generic;

namespace Game.Systems.Battler
{
    [Serializable]
    [SyncedComponent(typeof(IBattleComponentSyncedProperties))]

    public class BattleGroupComponent : IComponent, IBattleComponentSyncedProperties
    {
        public GameId BattleID { get; set; }
        public List<List<Unit>> UnitLines { get; set; } = new List<List<Unit>>()
        {
            new List<Unit>()
        };

        public List<Unit> FrontLine()
        {
            return UnitLines[0];
        }
    }
}

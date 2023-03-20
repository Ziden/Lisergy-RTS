using Game.DataTypes;
using Game.ECS;
using System;
using System.Collections.Generic;

namespace Game.Battler
{
    [Serializable]
    [SyncedComponent(logicType: typeof(IBattleComponentsLogic))]

    public class BattleGroupComponent : IComponent
    {
        public GameId BattleId;
        public List<List<Unit>> UnitLines = new List<List<Unit>>()
        {
            new List<Unit>()
        };

        public List<Unit> FrontLine()
        {
            return UnitLines[0];
        }

        static BattleGroupComponent()
        {
            SystemRegistry<BattleGroupComponent, WorldEntity>.AddSystem(new BattleGroupSystem());
        }
    }
}

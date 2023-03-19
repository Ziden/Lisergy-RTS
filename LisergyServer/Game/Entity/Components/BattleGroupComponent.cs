using Game.DataTypes;
using Game.ECS;
using Game.World.Systems;
using System;
using System.Collections.Generic;

namespace Game.Entity.Components
{
    [Serializable]
    [SyncedComponent]
    public class BattleGroupComponent : IComponent
    {
        public GameId BattleId;
        public List<List<Unit>> Units = new List<List<Unit>>()
        {
            new List<Unit>()
        };


        public List<Unit> FrontLine()
        {
            return Units[0];
        } 

        static BattleGroupComponent()
        {
            SystemRegistry<BattleGroupComponent, WorldEntity>.AddSystem(new BattleGroupSystem());
        }
    }
}

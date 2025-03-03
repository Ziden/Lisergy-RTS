using Game.Engine.ECLS;
using Game.Systems.Battler;
using GameData.Specs;
using System;

namespace Game.Systems.Dungeon
{
    [Serializable]
    [SyncedComponent]
    [RequiresComponent(typeof(BattleGroupComponent))]
    public class DungeonComponent : IComponent
    {
        public DungeonSpecId SpecId;

        public override string ToString() => $"<DungeonComponent Spec={SpecId}>";
    }
}

using Game.ECS;
using Game.Systems.Battler;
using System;

namespace Game.Systems.Dungeon
{
    [Serializable]
    [SyncedComponent]
    [RequiresComponent(typeof(BattleGroupComponent))]
    public struct DungeonComponent : IComponent
    {
        public ushort SpecId;

        public override string ToString() => $"<DungeonComponent Spec={SpecId}>";
    }
}

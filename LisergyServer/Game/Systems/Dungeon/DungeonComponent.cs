using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Inventories;
using System;

namespace Game.Systems.Dungeon
{
    [Serializable]
    [SyncedComponent]
    [RequiresComponent(typeof(BattleGroupComponent))]
    public class DungeonComponent : IComponent
    {
        public ushort SpecId;

        public override string ToString()
        {
            return $"<DungeonComponent Spec={SpecId}>";
        }
    }
}

using Game.ECS;
using Game.Entity.Components;
using Game.Inventories;
using System;

namespace Game.World.Components
{
    [Serializable]
    [SyncedComponent]
    [RequiresComponent(typeof(BattleGroupComponent))]
    public class DungeonComponent : IComponent
    {
        public ushort SpecId;

        [NonSerialized]
        public Item[] Rewards;
    }
}

using Game.ECS;
using Game.Entity.Entities;
using Game.Entity.Systems;
using Game.Inventories;
using System;

namespace Game.Entity.Components
{
    [Serializable]
    [SyncedComponent]
    [RequiresComponent(typeof(BattleGroupComponent))]
    public class DungeonComponent : IComponent
    {
        public ushort SpecId;

        [NonSerialized]
        public Item[] Rewards;

        static DungeonComponent()
        {
            SystemRegistry<DungeonComponent, DungeonEntity>.AddSystem(new DungeonSystem());
        }
    }
}

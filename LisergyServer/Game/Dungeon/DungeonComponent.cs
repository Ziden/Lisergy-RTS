using Game.Battler;
using Game.ECS;
using Game.Inventories;
using System;

namespace Game.Dungeon
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

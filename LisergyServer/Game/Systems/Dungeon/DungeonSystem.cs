using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Map;
using Game.Systems.Party;

namespace Game.Systems.Dungeon
{
    public class DungeonSystem : GameSystem<DungeonComponent>
    {
        public DungeonSystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {
            EntityEvents.On<GroupDeadEvent>(OnGroupDead);
        }

        private void OnGroupDead(IEntity e, DungeonComponent component, GroupDeadEvent ev)
        {
            GameLogic.Map(e).SetPosition(null);
        }
    }
}

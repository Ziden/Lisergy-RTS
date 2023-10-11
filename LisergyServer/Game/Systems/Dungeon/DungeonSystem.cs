using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Systems.Dungeon
{
    public class DungeonSystem : GameSystem<DungeonComponent>
    {
        public DungeonSystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {
            EntityEvents.On<GroupDeadEvent>(OnGroupDead);
        }

        private void OnGroupDead(IEntity e, GroupDeadEvent ev)
        {
            GameLogic.GetEntityLogic(e).Map.SetPosition(null);
        }
    }
}

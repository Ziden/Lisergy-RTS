using Game.Engine.ECS;
using Game.Systems.BattleGroup;
using Game.Systems.Map;

namespace Game.Systems.Party
{
    public class PartySystem : GameSystem<PartyComponent>
    {
        public PartySystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {
            EntityEvents.On<GroupDeadEvent>(OnGroupDead);
        }

        private void OnGroupDead(IEntity e, GroupDeadEvent ev)
        {
            e.EntityLogic.BattleGroup.Heal();
            var center = Players.GetPlayer(e.OwnerID).GetCenter().Get<MapPlacementComponent>();
            e.EntityLogic.Map.SetPosition(Game.World.Map.GetTile(center.Position.X, center.Position.Y));
        }
    }
}

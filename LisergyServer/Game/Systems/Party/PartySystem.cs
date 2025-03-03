using Game.Engine.ECLS;
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
            e.Logic.BattleGroup.Heal();

            var player = Players.GetPlayer(e.OwnerID);
            var center = player.EntityLogic.GetCenter();
            var centerLocation = center.Get<MapPlacementComponent>();
            e.Logic.Map.SetPosition(Game.World.GetTile(centerLocation.Position.X, centerLocation.Position.Y));
        }
    }
}

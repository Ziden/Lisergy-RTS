using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Map;
using Game.Systems.Movement;

namespace Game.Systems.Resources
{
    public unsafe class HarvestingSystem : LogicSystem<HarvesterComponent, HarvestingLogic>
    {
        public HarvestingSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {
            EntityEvents.On<ComponentUpdateEvent<MapPlacementComponent>>(OnUpdatePosition);
            EntityEvents.On<ComponentUpdateEvent<MovementComponent>>(OnUpdateMovementCourse);
        }

        private void OnUpdateMovementCourse(IEntity e, ComponentUpdateEvent<MovementComponent> ev)
        {
            if (ev.Old != null && ev.Old.CourseId != GameId.ZERO && ev.New?.CourseId == GameId.ZERO)
            {
                if (ev.Old.MovementIntent == CourseIntent.Harvest)
                {
                    var logic = GetLogic(e);
                    var tile = e.Logic.Map.GetTile();
                    if (logic.CanHarvest(tile)) logic.StartHarvesting(tile);
                }
            }
        }

        private void OnUpdatePosition(IEntity e, ComponentUpdateEvent<MapPlacementComponent> ev)
        {
            if (ev.Old != null)
            {
                var logic = GetLogic(e);
                if (logic.IsHarvesting()) logic.StopHarvesting();
            }
        }
    }
}

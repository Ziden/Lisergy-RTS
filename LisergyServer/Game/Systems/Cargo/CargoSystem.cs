using Game.ECS;
using Game.Systems.Harvesting;

namespace Game.Systems.Resources
{
    public unsafe class CargoSystem : LogicSystem<CargoComponent, CargoLogic>
    {
        public CargoSystem(LisergyGame game) : base(game)  {}

        public override void OnEnabled()
        {
            EntityEvents.On<HarvestingEndedEvent>(OnHarvestEnd);
        }

        private void OnHarvestEnd(IEntity entity, HarvestingEndedEvent ev)
        {
            if(ev.Resource.Amount > 0)
            {
                GetLogic(entity).AddTocargo(ev.Resource);
            }
        }
    }
}

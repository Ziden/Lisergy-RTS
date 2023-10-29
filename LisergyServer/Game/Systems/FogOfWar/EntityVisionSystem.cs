using Game.ECS;
using Game.Systems.BattleGroup;
using Game.Systems.Map;

namespace Game.Systems.FogOfWar
{
    public unsafe class EntityVisionSystem : LogicSystem<EntityVisionComponent, EntityVisionLogic>
    {
        public EntityVisionSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<EntityMoveInEvent>(OnEntityStepOnTile);
            EntityEvents.On<UnitAddToGroupEvent>(OnUnitAdded);
            EntityEvents.On<UnitRemovedEvent>(OnUnitRemoved);
        }

       
        private void OnUnitAdded(IEntity e, UnitAddToGroupEvent ev)
        {
            GetLogic(e).UpdateGroupLineOfSight();
        }

       
        private void OnUnitRemoved(IEntity e, UnitRemovedEvent ev)
        {
            GetLogic(e).UpdateGroupLineOfSight();
        }

       
        private void OnEntityStepOnTile(IEntity e, EntityMoveInEvent ev)
        {
            GetLogic(e).UpdateVisionRange(e, ev.FromTile, ev.ToTile);
        }
    }
}

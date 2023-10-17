using Game.ECS;
using Game.Events.GameEvents;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnUnitAdded(IEntity e, UnitAddToGroupEvent ev)
        {
            GetLogic(e).UpdateGroupLineOfSight();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnUnitRemoved(IEntity e, UnitRemovedEvent ev)
        {
            GetLogic(e).UpdateGroupLineOfSight();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnEntityStepOnTile(IEntity e, EntityMoveInEvent ev)
        {
            GetLogic(e).UpdateVisionRange(e, ev.FromTile, ev.ToTile);
        }
    }
}

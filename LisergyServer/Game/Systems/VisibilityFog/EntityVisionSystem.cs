using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.BattleGroup;
using Game.Systems.Map;

namespace Game.Systems.FogOfWar
{
    [SyncedSystem]
    public unsafe class EntityVisionSystem : LogicSystem<EntityVisionComponent, EntityVisionLogic>
    {
        public EntityVisionSystem(LisergyGame game) : base(game) { }
        public override void RegisterListeners()
        {
            EntityEvents.On<ComponentUpdateEvent<MapPlacementComponent>>(OnPlacementUpdate);
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

        private void OnPlacementUpdate(IEntity e, ComponentUpdateEvent<MapPlacementComponent> ev)
        {
            var oldTile = ev.Old == null ? null : Game.World.GetTile(ev.Old.Position);
            var newTIle = ev.New == null ? null : Game.World.GetTile(ev.New.Position);
            GetLogic(ev.Entity).UpdateVisionRange(oldTile, newTIle);
        }
    }
}

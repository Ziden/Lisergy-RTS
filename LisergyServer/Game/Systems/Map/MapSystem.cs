using Game.Engine.ECLS;
using Game.Engine.Events;

namespace Game.Systems.Map
{
    [SyncedSystem]
    public class MapSystem : LogicSystem<MapPlaceableComponent, MapLogic>
    {
        public MapSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {
            EntityEvents.On<ComponentUpdateEvent<MapPlacementComponent>>(OnPlacementUpdate);
        }

        private void OnPlacementUpdate(IEntity e, ComponentUpdateEvent<MapPlacementComponent> ev)
        {
            var oldTile = ev.Old == null ? null : Game.World.GetTile(ev.Old.Position);
            var newTile = ev.New == null ? null : Game.World.GetTile(ev.New.Position);
            if (oldTile != null)
            {
                oldTile.Logic.Map.RemoveEntityFromTile(e);
            }
            if (newTile != null)
            {
                newTile.Logic.Map.SetEntityOnTile(e);
            }
        }
    }
}

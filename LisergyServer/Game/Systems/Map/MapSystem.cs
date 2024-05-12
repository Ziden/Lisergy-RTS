using Game.ECS;
using Game.Engine.ECS;
using Game.Systems.Tile;

namespace Game.Systems.Map
{
    [SyncedSystem]
    public class MapSystem : LogicSystem<MapPlacementComponent, MapLogic>
    {
        public MapSystem(LisergyGame game) : base(game) { }

        public override void RegisterListeners()
        {
            EntityEvents.On<EntityMoveOutEvent>(OnEntityMoveOut);
            EntityEvents.On<EntityMoveInEvent>(OnEntityMoveIn);
        }

        private void OnEntityMoveOut(IEntity owner, EntityMoveOutEvent ev)
        {
            ev.FromTile.Components.GetReference<TileHabitantsReferenceComponent>().EntitiesIn.Remove(ev.Entity);
        }

        private void OnEntityMoveIn(IEntity owner, EntityMoveInEvent ev)
        {
            var tileHabitants = ev.ToTile.Components.GetReference<TileHabitantsReferenceComponent>();
            tileHabitants.EntitiesIn.Add(ev.Entity);
        }
    }
}

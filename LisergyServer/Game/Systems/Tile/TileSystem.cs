using Game.ECS;
using Game.Systems.Building;
using Game.Systems.Map;

namespace Game.Systems.Tile
{
    /// <summary>
    /// System that keeps track of references of which entities and which buildings are on which tiles
    /// </summary>
    [SyncedSystem]
    public class TileSystem : GameSystem<TileComponent>
    {
        public TileSystem(LisergyGame game) : base(game) { }
        public override void RegisterListeners()
        {
            EntityEvents.On<BuildingPlacedEvent>(OnStaticEntityPlaced);
            EntityEvents.On<BuildingRemovedEvent>(OnStaticEntityRemoved);

        }

        private void OnStaticEntityRemoved(IEntity owner, BuildingRemovedEvent entity)
        {
            owner.Components.GetReference<TileHabitantsReferenceComponent>().Building = null;
        }

        private void OnStaticEntityPlaced(IEntity owner, BuildingPlacedEvent ev)
        {
            owner.Components.GetReference<TileHabitantsReferenceComponent>().Building = ev.Entity;
        }
    }
}

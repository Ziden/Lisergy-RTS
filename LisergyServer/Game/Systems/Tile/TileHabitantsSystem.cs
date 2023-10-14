using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;

namespace Game.Systems.Tile
{
    /// <summary>
    /// System that keeps track of references of which entities and which buildings are on which tiles
    /// </summary>
    public class TileHabitantsSystem : GameSystem<TileComponent>
    {
        public TileHabitantsSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<BuildingPlacedEvent>(OnStaticEntityPlaced);
            EntityEvents.On<BuildingRemovedEvent>(OnStaticEntityRemoved);
            EntityEvents.On<EntityMoveOutEvent>(OnEntityMoveOut);
            EntityEvents.On<EntityMoveInEvent>(OnEntityMoveIn);
        }

        private void OnStaticEntityRemoved(IEntity owner, BuildingRemovedEvent entity)
        {
            owner.Components.GetReference<TileHabitants>().Building = null;
        }

        private void OnStaticEntityPlaced(IEntity owner, BuildingPlacedEvent ev)
        {
            owner.Components.GetReference<TileHabitants>().Building = ev.Entity;
        }

        private void OnEntityMoveOut(IEntity owner, EntityMoveOutEvent ev)
        {
            owner.Components.GetReference<TileHabitants>().EntitiesIn.Remove(ev.Entity);
        }

        private void OnEntityMoveIn(IEntity owner, EntityMoveInEvent ev)
        {
            var tileHabitants = owner.Components.GetReference<TileHabitants>();
            tileHabitants.EntitiesIn.Add(ev.Entity);
        }
    }
}

using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.Movement;

namespace Game.Systems.Tile
{
    public class TileHabitantsSystem : GameSystem<TileHabitants>
    {
        public TileHabitantsSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<BuildingPlacedEvent>(OnStaticEntityPlaced);
            EntityEvents.On<BuildingRemovedEvent>(OnStaticEntityRemoved);
            EntityEvents.On<EntityMoveOutEvent>(OnEntityMoveOut);
            EntityEvents.On<EntityMoveInEvent>(OnEntityMoveIn);
        }

        private void OnStaticEntityRemoved(IEntity owner, TileHabitants component, BuildingRemovedEvent entity)
        {
            component.Building = null;
        }

        private void OnStaticEntityPlaced(IEntity owner, TileHabitants component, BuildingPlacedEvent ev)
        {
            component.Building = ev.Entity;
        }

        private void OnEntityMoveOut(IEntity owner, TileHabitants component, EntityMoveOutEvent ev)
        {
            component.EntitiesIn.Remove(ev.Entity);
        }

        private void OnEntityMoveIn(IEntity owner, TileHabitants tileHabitants, EntityMoveInEvent ev)
        {
            var movement = ev.Entity.Components.Get<EntityMovementComponent>();
            if (movement == null)
            {
                return;
            }
            tileHabitants.EntitiesIn.Add(ev.Entity);
            var atkGroup = ev.Entity.Components.Get<BattleGroupComponent>();
            if (atkGroup == null) return;
            if (tileHabitants.Building == null) return;
            var defGroup = tileHabitants.Building.Components.Get<BattleGroupComponent>();
            if(defGroup == null) return;    
            if (movement.Course != null && movement.Course.Intent == MovementIntent.Offensive && movement.Course.IsLastMovement())
            {
                ev.Entity.Components.CallEvent(new OffensiveActionEvent()
                {
                    AttackerGroup = atkGroup,
                    DefenderGroup = defGroup,
                    Defender = tileHabitants.Building,
                    Attacker = ev.Entity
                });
            }
        }
    }
}

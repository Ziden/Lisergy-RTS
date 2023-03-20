using Game.Battler;
using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.FogOfWar;

namespace Game.Party
{
    public class PartySystem : GameSystem<PartyComponent, PartyEntity>
    {
        internal override void OnComponentAdded(PartyEntity owner, PartyComponent component, EntityEventBus events)
        {
            owner.BattleGroupLogic.OnUnitAdded += OnUnitAdded;
            owner.BattleGroupLogic.OnUnitRemoved += OnUnitRemoved;
            events.RegisterComponentEvent<PartyEntity, BattleFinishedEvent, PartyComponent>(OnBattleFinished);
        }

        private static void OnBattleFinished(PartyEntity e, PartyComponent p, BattleFinishedEvent ev)
        {
            var battleable = (IBattleableEntity)e;
            battleable.BattleGroupLogic.BattleID = GameId.ZERO;
            if (battleable.BattleGroupLogic.IsDestroyed)
            {
                e.Tile = e.Owner.GetCenter().Tile;
                foreach (var unit in battleable.BattleGroupLogic.GetUnits())
                    unit.HealAll();
            }
        }

        private static void OnUnitAdded(IBattleComponentsLogic logic, UnitAddedEvent ev)
        {
            var c = ev.Entity.Components.Get<EntityVisionComponent>();
            c.LineOfSight = logic.CalculateLineOfSight();
        }

        private static void OnUnitRemoved(IBattleComponentsLogic logic, UnitRemovedEvent ev)
        {
            var c = ev.Entity.Components.Get<EntityVisionComponent>();
            c.LineOfSight = logic.CalculateLineOfSight();
        }
    }
}

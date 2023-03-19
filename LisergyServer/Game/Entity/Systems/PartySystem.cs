using Game.DataTypes;
using Game.ECS;
using Game.Entity.Components;
using Game.Entity.Entities;
using Game.Entity.Logic;
using Game.Events.GameEvents;

namespace Game.World.Systems
{
    public class PartySystem : GameSystem<PartyComponent, PartyEntity>
    {
        internal override void OnComponentAdded(PartyEntity owner, PartyComponent component, EntityEventBus events)
        {
            events.RegisterComponentEvent<PartyEntity, UnitAddedEvent, PartyComponent>(OnUnitAdded);
            events.RegisterComponentEvent<PartyEntity, UnitRemovedEvent, PartyComponent>(OnUnitRemoved);
            events.RegisterComponentEvent<PartyEntity, UnitRemovedEvent, PartyComponent>(OnUnitRemoved);
            events.RegisterComponentEvent<PartyEntity, BattleFinishedEvent, PartyComponent>(OnBattleFinished);
        }

        private static void OnBattleFinished(PartyEntity e, PartyComponent p, BattleFinishedEvent ev)
        {
            var battleable = (IBattleableEntity)e;
            battleable.BattleLogic.BattleID = GameId.ZERO;
            if (battleable.BattleLogic.IsDestroyed)
            {
                e.Tile = e.Owner.GetCenter().Tile;
                foreach (var unit in battleable.BattleLogic.GetUnits())
                    unit.HealAll();
            }
        }

        private static void OnUnitAdded(PartyEntity e, PartyComponent party, UnitAddedEvent ev)
        {
            var c = e.Components.Get<EntityVisionComponent>();
            c.LineOfSight = BattleGroupSystem.CalculateGetGroupLOS(e);
        }

        private static void OnUnitRemoved(PartyEntity e, PartyComponent party, UnitRemovedEvent ev)
        {
            var c = e.Components.Get<EntityVisionComponent>();
            c.LineOfSight = BattleGroupSystem.CalculateGetGroupLOS(e);
        }
    }
}

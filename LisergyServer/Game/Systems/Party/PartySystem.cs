using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;

namespace Game.Systems.Party
{
    public class PartySystem : GameSystem<PartyComponent, PartyEntity>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<BattleFinishedEvent>(OnBattleFinished);
        }

        internal override void OnComponentAdded(PartyEntity owner, PartyComponent component)
        {
            owner.BattleGroupLogic.OnUnitAdded += OnUnitAdded;
            owner.BattleGroupLogic.OnUnitRemoved += OnUnitRemoved;
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

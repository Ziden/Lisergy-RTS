using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;

namespace Game.Systems.Party
{
    public class PartySystem : GameSystem<PartyComponent>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<BattleFinishedEvent>(OnBattleFinished);
        }

        internal override void OnComponentAdded(IEntity owner, PartyComponent component)
        {
            var p = (PartyEntity)owner;
            p.BattleGroupLogic.OnUnitAdded += OnUnitAdded;
            p.BattleGroupLogic.OnUnitRemoved += OnUnitRemoved;
        }

        private static void OnBattleFinished(IEntity e, PartyComponent p, BattleFinishedEvent ev)
        {
            var party = (PartyEntity)e;
            var battleable = (IBattleableEntity)e;
            battleable.BattleGroupLogic.BattleID = GameId.ZERO;
            if (battleable.BattleGroupLogic.IsDestroyed)
            {
                party.Tile = e.Owner.GetCenter().Tile;
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

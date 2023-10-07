using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Network.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Battler
{
    public class BattleGroupLogic : BaseEntityLogic<BattleGroupComponent>
    {
        public bool IsBattling => !Entity.Get<BattleGroupComponent>().BattleID.IsZero();

        public bool IsDestroyed => GetUnits().All(u => u == null || u.HP <= 0);

        public Unit Leader => GetUnits().First();

        public void UpdateUnits(List<Unit> newUnits)
        {
            var component = Entity.Get<BattleGroupComponent>();
            var units = component.Units;
            for (var x = 0; x < Math.Max(newUnits.Count, units.Count()); x++)
            {
                if (x >= units.Count)
                {
                    AddUnit(newUnits[x]);
                }
                else if (x >= newUnits.Count)
                {
                    RemoveUnit(units[x], x);
                }
                else
                {
                    var oldUnit = units[x];
                    var newUnit = newUnits[x];
                    if (!oldUnit.Equals(newUnit))
                    {
                        ReplaceUnit(oldUnit, newUnit, x);
                    }
                }
            }
        }

        public IEnumerable<Unit> GetValidUnits() => GetUnits().Where(u => u != null);

        public IReadOnlyList<Unit> GetUnits() => Entity.Get<BattleGroupComponent>().Units;

        public virtual void ReplaceUnit(Unit oldUnit, Unit newUnit, int preferAtIndex = -1)
        {
            var component = Entity.Get<BattleGroupComponent>();
            var units = component.Units;
            if (preferAtIndex == -1)
            {
                preferAtIndex = units.IndexOf(oldUnit);
                RemoveUnit(oldUnit);
                AddUnit(newUnit, preferAtIndex);
            }
            else
            {
                RemoveUnit(oldUnit, preferAtIndex);
                AddUnit(newUnit, preferAtIndex);
            }
        }

        public virtual void AddUnit(Unit u, int preferAtIndex = -1)
        {
            var component = Entity.Get<BattleGroupComponent>();
            var units = component.Units;
            if (preferAtIndex >= 0) units.Insert(preferAtIndex, u);
            else units.Add(u);
            Entity.Components.Save(component);
            Entity.Components.CallEvent(new UnitAddToGroupEvent(Entity, component, u));
        }

        public virtual void RemoveUnit(Unit u, int preferAtIndex = -1)
        {
            var component = Entity.Get<BattleGroupComponent>();
            var units = component.Units;
            if (!units.Contains(u))
            {
                throw new Exception($"Trying to remove unit {u} to entity {Entity} but unit was not there");
            }
            if (preferAtIndex != -1)
            {
                if (!units[preferAtIndex].Equals(u))
                {
                    throw new Exception("Removing unit from wrong index");
                }
                units.RemoveAt(preferAtIndex);
            }
            else
            {
                units.Remove(u);
            }
            Entity.Components.Save(component);
            Entity.Components.CallEvent(new UnitRemovedEvent(Entity, component, u));
        }

        public void ClearBattleId()
        {
            var attackerComponent = Entity.Get<BattleGroupComponent>();
            attackerComponent.BattleID = GameId.ZERO;
            Entity.Components.Save(attackerComponent);
        }

        public GameId StartBattle(IEntity defender)
        {
            var battleID = Guid.NewGuid();
            var battleEvent = new BattleTriggeredEvent(battleID, Entity, defender);
            var attackerComponent = Entity.Get<BattleGroupComponent>();
            attackerComponent.BattleID = battleID;
            Entity.Components.Save(attackerComponent);
            var defenderComponent = defender.Get<BattleGroupComponent>();
            defenderComponent.BattleID = battleID;
            defender.Components.Save(defenderComponent);
            Game.Events.Call(battleEvent);
            return battleID;
        }

        public BattleTeam GetBattleTeam() => new BattleTeam(Entity, GetUnits().ToArray());
    }
}

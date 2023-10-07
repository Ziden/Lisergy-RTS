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
        public bool IsBattling => !Component.BattleID.IsZero();

        public bool IsDestroyed => GetUnits().All(u => u == null || u.HP <= 0);

        public Unit Leader => GetUnits().First();

        public void UpdateUnits(List<Unit> newUnits)
        {
            var units = Component.Units;
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

        public IReadOnlyList<Unit> GetUnits() => Component.Units;

        public virtual void ReplaceUnit(Unit oldUnit, Unit newUnit, int preferAtIndex = -1)
        {
            var units = Component.Units;
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
            var units = Component.Units;
            if (preferAtIndex >= 0)
            {
                units.Insert(preferAtIndex, u);
            }
            else
            {
                units.Add(u);
            }
            Entity.Components.CallEvent(new UnitAddToGroupEvent(Entity, Component, u));
        }

        public virtual void RemoveUnit(Unit u, int preferAtIndex = -1)
        {
            var units = Component.Units;
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
            Entity.Components.CallEvent(new UnitRemovedEvent(Entity, Component, u));
        }

        public GameId StartBattle(IEntity defender)
        {
            var battleID = Guid.NewGuid();
            var battleEvent = new BattleTriggeredEvent(battleID, Entity, defender);
            Component.BattleID = battleID;
            defender.Get<BattleGroupComponent>().BattleID = battleID;
            Game.Events.Call(battleEvent);
            return battleID;
        }

        public BattleTeam GetBattleTeam() => new BattleTeam(Entity, GetUnits().ToArray());
    }
}

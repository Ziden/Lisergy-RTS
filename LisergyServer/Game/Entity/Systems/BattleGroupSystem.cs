
using Game.DataTypes;
using Game.ECS;
using Game.Entity.Components;
using Game.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.World.Systems
{
    public class BattleGroupSystem : GameSystem<BattleGroupComponent, WorldEntity>
    {
        public delegate void Test(IEntity entity);

        internal override void OnComponentAdded(WorldEntity owner, BattleGroupComponent component, EntityEventBus events)
        {
            
        }

        public static void NewUnitLine(WorldEntity e)
        {
            var comp = e.Components.Get<BattleGroupComponent>();
            comp.UnitLines.Insert(0, new List<Unit>());
        }

        public static void RemoveUnit(WorldEntity e, Unit u, int preferAtIndex=-1)
        {
            var units = e.Components.Get<BattleGroupComponent>().FrontLine();
            if (!units.Contains(u))
            {
                throw new Exception($"Trying to remove unit {u} to entity {e} but unit was not there");
            }
            if(preferAtIndex != -1)
            {
                if (!units[preferAtIndex].Equals(u))
                {
                    throw new Exception("Removing unit from wrong index");
                }
                units.RemoveAt(preferAtIndex);
            } else
            {
                units.Remove(u);
            }
            e.Components.CallEvent(new UnitRemovedEvent(e, u));
        }

        public static void ReplaceUnit(WorldEntity e, Unit oldUnit, Unit newUnit, int preferAtIndex = -1)
        {
            var units = e.Components.Get<BattleGroupComponent>().FrontLine();
            if(preferAtIndex == -1)
            {
                preferAtIndex = units.IndexOf(oldUnit);
                RemoveUnit(e, oldUnit);
                AddUnit(e, newUnit, preferAtIndex);
            } else
            {
                RemoveUnit(e, oldUnit, preferAtIndex);
                AddUnit(e, newUnit, preferAtIndex);
            }
        }

        public static void UpdateUnits(WorldEntity e, List<Unit> newUnits)
        {
            var units = e.Components.Get<BattleGroupComponent>().FrontLine();
            for (var x = 0; x < Math.Max(newUnits.Count, units.Count()); x++)
            {
                if (x >= units.Count)
                {
                    AddUnit(e, newUnits[x]);
                }
                else if (x >= newUnits.Count)
                {
                    RemoveUnit(e, units[x], x);
                }
                else
                {
                    var oldUnit = units[x];
                    var newUnit = newUnits[x];
                    if (!oldUnit.Equals(newUnit))
                    {
                        ReplaceUnit(e, oldUnit, newUnit, x);
                    }
                }
            }
        }

        public static void AddUnit(WorldEntity e, Unit u, int preferAtIndex = -1)
        {
            var units = e.Components.Get<BattleGroupComponent>().FrontLine();
            if(preferAtIndex >= 0)
            {
                units.Insert(preferAtIndex, u);
            } else
            {
                units.Add(u);
            }
            e.Components.CallEvent(new UnitAddedEvent(e, u));
        }

        public static void SetBattleId(WorldEntity e, GameId battleId) => e.Components.Get<BattleGroupComponent>().BattleId = battleId;

        public static GameId GetBattleId(WorldEntity e) => e.Components.Get<BattleGroupComponent>()?.BattleId ?? GameId.ZERO;

        public static byte CalculateGetGroupLOS(WorldEntity e) {
            byte max = 0;
            foreach(var unit in GetUnits(e))
            {
                if(unit != null && unit.GetSpec().LOS > max)
                {
                    max = unit.GetSpec().LOS;
                }
            }
            return max;
         }

        public static bool IsBattleable(WorldEntity e) => e.Components.Has<BattleGroupComponent>();

        public static IReadOnlyList<Unit> GetUnits(WorldEntity e) => e.Components.Get<BattleGroupComponent>().FrontLine();

        public static bool IsAlive(WorldEntity e) => GetUnits(e).Where(u => u != null && u.HP > 0).Any();
    }
}

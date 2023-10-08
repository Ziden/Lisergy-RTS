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

        public bool IsDestroyed => Entity.Get<BattleGroupComponent>().Units.AllDead;

        public void UpdateUnits(Unit [] newUnits)
        {
            if (newUnits.Length != 4) throw new Exception("Need 4 units");
            var component = Entity.Get<BattleGroupComponent>();
            for(var x = 0; x < 4; x++)
            {
                if (component.Units[x].Valid) RemoveUnit(component.Units[x], x);
                AddUnit(newUnits[x], x);
            }
        }

        public void Heal()
        {
            var component = Entity.Get<BattleGroupComponent>();
            component.Units.HealAll();
            Entity.Components.Save(component);
        }

        public virtual void ReplaceUnit(in Unit oldUnit, in Unit newUnit, in int preferAtIndex = -1)
        {
            RemoveUnit(oldUnit, preferAtIndex);
            AddUnit(newUnit, preferAtIndex);
        }

        /*
        public unsafe void UpdateFrom(BattleTeam team)
        {
            for(var x = 0; x < team.Units.Length; x++)
            {
                var size = sizeof(Unit);
                var sourcePtr = &stats;
                fixed (UnitStats* thisPtr = &this)
                {
                    Buffer.MemoryCopy(sourcePtr, thisPtr, size, size);
                }
            }
        }
        */

        public virtual void AddUnit(in Unit u, in int preferAtIndex = -1)
        {
            var component = Entity.Get<BattleGroupComponent>();
            if (preferAtIndex >= 0) component.Units[preferAtIndex] = u;
            else component.Units.Add(u);
            Entity.Components.Save(component);
            Entity.Components.CallEvent(new UnitAddToGroupEvent(Entity, component, u));
        }

        public virtual void RemoveUnit(in Unit u, in int preferAtIndex = -1)
        {
            var component = Entity.Get<BattleGroupComponent>();
            if (!component.Units.Contains(u))
            {
                throw new Exception($"Trying to remove unit {u} to entity {Entity} but unit was not there");
            }
            if (preferAtIndex != -1)
            {
                if (!component.Units[preferAtIndex].Equals(u))
                {
                    throw new Exception("Removing unit from wrong index");
                }
                component.Units[preferAtIndex] = default;
            }
            else
            {
                component.Units.Remove(u);
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
            GameId battleID = GameId.Generate();
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
    }
}

using Game.Engine.DataTypes;
using Game.Engine.ECS;
using Game.Systems.BattleGroup;
using System;

namespace Game.Systems.Battler
{
    public unsafe class BattleGroupLogic : BaseEntityLogic<BattleGroupComponent>
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
            var component = Entity.Components.GetPointer<BattleGroupComponent>();
            component->Units.HealAll();
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
            var component = Entity.Components.GetPointer<BattleGroupComponent>();
            if (preferAtIndex >= 0) component->Units[preferAtIndex] = u;
            else component->Units.Add(u);
            Entity.Components.CallEvent(new UnitAddToGroupEvent()
            {
                Unit = u,
                Entity = Entity
            });
        }

        public virtual void RemoveUnit(in Unit u, in int preferAtIndex = -1)
        {
            var component = Entity.Components.GetPointer<BattleGroupComponent>();
            if (!component->Units.Contains(u))
            {
                throw new Exception($"Trying to remove unit {u} to entity {Entity} but unit was not there");
            }
            if (preferAtIndex != -1)
            {
                if (!component->Units[preferAtIndex].Equals(u))
                {
                    throw new Exception("Removing unit from wrong index");
                }
                component->Units[preferAtIndex] = default;
            }
            else
            {
                component->Units.Remove(u);
            }
            var component2 = Entity.Components.GetPointer<BattleGroupComponent>();
            Entity.Components.CallEvent(new UnitRemovedEvent(Entity, u));
        }

        public void ClearBattleId()
        {
            Entity.Components.GetPointer<BattleGroupComponent>()->BattleID = GameId.ZERO;
        }

        public GameId StartBattle(IEntity defender)
        {
            GameId battleID = GameId.Generate();
            var battleEvent = new BattleTriggeredEvent(battleID, Entity, defender);
            Entity.Components.GetPointer<BattleGroupComponent>()->BattleID = battleID;
            defender.Components.GetPointer<BattleGroupComponent>()->BattleID = battleID;
            Game.Events.Call(battleEvent);
            return battleID;
        }
    }
}

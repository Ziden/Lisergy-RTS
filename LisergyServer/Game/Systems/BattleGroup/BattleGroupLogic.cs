using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.BattleGroup;
using System;

namespace Game.Systems.Battler
{
    public unsafe class BattleGroupLogic : BaseEntityLogic<BattleGroupComponent>
    {
        public bool IsBattling => !CurrentEntity.Get<BattleGroupComponent>().BattleID.IsZero();

        public bool IsDestroyed => CurrentEntity.Get<BattleGroupComponent>().Units.AllDead;

        public void UpdateUnits(Unit[] newUnits)
        {
            if (newUnits.Length != 4) throw new Exception("Need 4 units");
            var component = CurrentEntity.Get<BattleGroupComponent>();
            for (var x = 0; x < 4; x++)
            {
                if (component.Units[x].Valid) RemoveUnit(component.Units[x], x);
                AddUnit(newUnits[x], x);
            }
        }

        public void Heal()
        {
            var component = CurrentEntity.Components.Get<BattleGroupComponent>();
            component.Units.HealAll();
            CurrentEntity.Save(component);
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
            var component = CurrentEntity.Components.Get<BattleGroupComponent>();
            if (preferAtIndex >= 0) component.Units[preferAtIndex] = u;
            else component.Units.Add(u);
            CurrentEntity.Save(component);
            CurrentEntity.Components.CallEvent(new UnitAddToGroupEvent()
            {
                Unit = u,
                Entity = CurrentEntity
            });
        }

        public virtual void RemoveUnit(in Unit u, in int preferAtIndex = -1)
        {
            var component = CurrentEntity.Components.Get<BattleGroupComponent>();
            if (!component.Units.Contains(u))
            {
                throw new Exception($"Trying to remove unit {u} to entity {CurrentEntity} but unit was not there");
            }
            if (preferAtIndex != -1)
            {
                if (!component.Units[preferAtIndex].Equals(u))
                {
                    throw new Exception("Removing unit from wrong index");
                }
                component.Units[preferAtIndex] = default;
                CurrentEntity.Save(component);
            }
            else
            {
                component.Units.Remove(u);
                CurrentEntity.Save(component);
            }
            CurrentEntity.Components.CallEvent(new UnitRemovedEvent(CurrentEntity, u));
        }

        public void ClearBattleId()
        {
            var c = CurrentEntity.Components.Get<BattleGroupComponent>();
            c.BattleID = GameId.ZERO;
            CurrentEntity.Components.Save(c);
        }

        public GameId StartBattle(IEntity defender)
        {
            GameId battleID = GameId.Generate();
            var battleEvent = new BattleTriggeredEvent(battleID, CurrentEntity, defender);
            var c1 = CurrentEntity.Components.Get<BattleGroupComponent>();
            c1.BattleID = battleID;
            CurrentEntity.Save(c1);
            var c2 = defender.Components.Get<BattleGroupComponent>();
            c2.BattleID = battleID;
            defender.Save(c2);
            Game.Events.Call(battleEvent);
            return battleID;
        }
    }
}

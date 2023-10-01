using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Entity;
using Game.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Battler
{

    public interface IBattleableEntity : IOwnable, IMapEntity, IStatusUpdateable
    {
        public IBattleComponentsLogic BattleGroupLogic { get; }
    }

    /// <summary>
    /// Internal logic events
    /// </summary>
    public interface IBattleComponentEvents
    {
        public event Action<IBattleComponentsLogic, GameId> OnBattleIdChanged;
        public event Action<IBattleComponentsLogic, UnitAddedEvent> OnUnitAdded;
        public event Action<IBattleComponentsLogic, UnitRemovedEvent> OnUnitRemoved;
    }

    /// <summary>
    /// Sync logic will sync properties
    /// Synced properties are defined here.
    /// </summary>
    public interface IBattleComponentSyncedProperties
    {
        public GameId BattleID { get; set; }
        public List<List<Unit>> UnitLines { set; }
    }

    public interface IBattleComponentsLogic : IBattleComponentSyncedProperties, IBattleComponentEvents, IComponentEntityLogic
    {
        bool IsBattling { get; }
        void UpdateUnits(List<Unit> newUnits);
        IEnumerable<Unit> GetValidUnits();
        IReadOnlyList<Unit> GetUnits();
        void ReplaceUnit(Unit oldUnit, Unit newUnit, int index = -1);
        void AddUnit(Unit u, int prefferedIndex = -1);
        void RemoveUnit(Unit u, int preferAtIndex = -1);
        BattleTeam GetBattleTeam();
        bool IsDestroyed { get; }
        void NewUnitLine();
        byte CalculateLineOfSight();
        public Unit Leader { get; }
    }

    public class BattleGroupComponentLogic : IBattleComponentsLogic
    {
        public event Action<IBattleComponentsLogic, GameId> OnBattleIdChanged;
        public event Action<IBattleComponentsLogic, UnitAddedEvent> OnUnitAdded;
        public event Action<IBattleComponentsLogic, UnitRemovedEvent> OnUnitRemoved;

        public IEntity Entity { get => _entity; set => _entity = (WorldEntity)value; }
        public IComponent Component { get; set; }

        private IEntity _entity;
        private IBattleableEntity _battleable;
        private BattleGroupComponent _component => Entity.Components.Get<BattleGroupComponent>();

        public BattleGroupComponentLogic(WorldEntity battler)
        {
            _entity = battler;
            _battleable = _entity as IBattleableEntity;
        }

        #region Synced Properties
        public virtual GameId BattleID
        {
            get => _component?.BattleID ?? GameId.ZERO;
            set
            {
                _component.BattleID = value;
                OnBattleIdChanged?.Invoke(this, value);
            }
        }
        #endregion

        public void NewUnitLine()
        {
            var comp = _component;
            comp.UnitLines.Insert(0, new List<Unit>());
        }

        public bool IsBattling => !BattleID.IsZero();

        public bool IsDestroyed => GetUnits().All(u => u == null || u.HP <= 0);

        public List<List<Unit>> UnitLines
        {
            set
            {
                // Update frontline to trigger events
                UpdateUnits(value.First());
                // Update rest
                for (int x = 1; x < value.Count; x++)
                    _component.UnitLines.Add(value[x]);
            }
        }

        public Unit Leader => GetUnits().First();

        public void UpdateUnits(List<Unit> newUnits)
        {
            var units = _component.FrontLine();
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

        public IReadOnlyList<Unit> GetUnits() => _component.FrontLine();

        public virtual void ReplaceUnit(Unit oldUnit, Unit newUnit, int preferAtIndex = -1)
        {
            var units = _component.FrontLine();
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

        public byte CalculateLineOfSight()
        {
            byte max = 0;
            foreach (var unit in GetUnits())
            {
                if (unit != null && unit.GetSpec().LOS > max)
                {
                    max = unit.GetSpec().LOS;
                }
            }
            return max;
        }

        public virtual void AddUnit(Unit u, int preferAtIndex = -1)
        {
            var units = _component.FrontLine();
            if (preferAtIndex >= 0)
            {
                units.Insert(preferAtIndex, u);
            }
            else
            {
                units.Add(u);
            }
            OnUnitAdded?.Invoke(this, new UnitAddedEvent(_entity, u));
        }

        public virtual void RemoveUnit(Unit u, int preferAtIndex = -1)
        {
            var units = _component.FrontLine();
            if (!units.Contains(u))
            {
                throw new Exception($"Trying to remove unit {u} to entity {_entity} but unit was not there");
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
            OnUnitRemoved?.Invoke(this, new UnitRemovedEvent(_entity, u));
        }

        public BattleTeam GetBattleTeam() => new BattleTeam(_battleable, GetUnits().ToArray());
    }
}

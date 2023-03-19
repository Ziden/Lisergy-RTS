using Game.Battles;
using Game.DataTypes;
using Game.Entity.Components;
using Game.World.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entity.Logic
{

    public interface IBattleableEntity : IOwnable, IMapEntity, IStatusUpdateable
    {
        public IBattleComponentsLogic BattleLogic { get; }
    }

    public interface IBattleComponentsLogic
    {
        public GameId BattleID { get; set; }
        bool IsBattling { get; }
        void UpdateUnits(List<Unit> newUnits);
        IEnumerable<Unit> GetValidUnits();
        IReadOnlyList<Unit> GetUnits();
        void ReplaceUnit(Unit oldUnit, Unit newUnit, int index = -1);
        void AddUnit(Unit u);
        void RemoveUnit(Unit u);
        BattleTeam GetBattleTeam();
        bool IsDestroyed { get; }
        void NewUnitLine();

    }

    public class BattleComponentsLogic : IBattleComponentsLogic
    {
        private WorldEntity _entity;
        private IBattleableEntity _battleable;

        public BattleComponentsLogic(WorldEntity battler)
        {
            if (!battler.Components.Has<BattleGroupComponent>())
            {
                throw new Exception("To use battle logic needs battle group component");
            }
            _entity = battler;
            _battleable = _entity as IBattleableEntity;
        }

        public virtual GameId BattleID
        {
            get => BattleGroupSystem.GetBattleId(_entity);
            set => BattleGroupSystem.SetBattleId(_entity, value);
        }

        public void NewUnitLine() => BattleGroupSystem.NewUnitLine(_entity);

        public bool IsBattling => !BattleID.IsZero();

        public bool IsDestroyed => GetUnits().All(u => u == null || u.HP <= 0);

        public void UpdateUnits(List<Unit> newUnits) => BattleGroupSystem.UpdateUnits(_entity, newUnits);

        public IEnumerable<Unit> GetValidUnits() => GetUnits().Where(u => u != null);

        public IReadOnlyList<Unit> GetUnits() => BattleGroupSystem.GetUnits(_entity);

        public virtual void ReplaceUnit(Unit oldUnit, Unit newUnit, int index = -1) => BattleGroupSystem.ReplaceUnit(_entity, oldUnit, newUnit, index);

        public virtual void AddUnit(Unit u) => BattleGroupSystem.AddUnit(_entity, u);

        public virtual void RemoveUnit(Unit u) => BattleGroupSystem.RemoveUnit(_entity, u);

        public BattleTeam GetBattleTeam() => new BattleTeam(_battleable, BattleGroupSystem.GetUnits(_entity).ToArray());
    }
}

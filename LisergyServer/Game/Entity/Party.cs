using Game.Events;
using Game.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Battles;
using Game.Battle;
using Game.Events.ServerEvents;
using Game.Entity.Components;
using Game.Movement;
using Game.World.Systems;
using Game.DataTypes;

namespace Game.Entity
{
    [Serializable]
    public class Party : WorldEntity, IBattleable
    {
        public Party(PlayerEntity owner, byte partyIndex) : base(owner)
        {
            SetupDefaultComponents();
            PartyIndex = partyIndex;
        }

        public Party(PlayerEntity owner) : base(owner)
        {
            SetupDefaultComponents();
        }

        public bool IsAlive() => BattleGroupSystem.IsAlive(this);

        public bool CanMove()
        {
            return !IsBattling;
        }

        public CourseTask Course
        {
            get => EntityMovementSystem.GetCourse(this);
            set => EntityMovementSystem.SetCourse(this, value);
        }

        public virtual GameId BattleID
        {
            get => BattleGroupSystem.GetBattleId(this);
            set => BattleGroupSystem.SetBattleId(this, value);
        }

        public bool IsBattling => !BattleID.IsZero();

        private void SetupDefaultComponents()
        {
            this.Components.Add(new PartyComponent());
            this.Components.Add(new BattleGroupComponent());
            this.Components.Add(new EntityExplorationComponent());
            this.Components.Add(new EntityMovementComponent() { MoveDelay = TimeSpan.FromSeconds(0.25) });
        }

        public void UpdateUnits(List<Unit> newUnits) => BattleGroupSystem.UpdateUnits(this, newUnits);

        public byte PartyIndex { get => Components.Get<PartyComponent>().PartyIndex; set => Components.Get<PartyComponent>().PartyIndex = value; }

        public byte GetLineOfSight() => Components.Get<EntityExplorationComponent>()?.LineOfSight ?? 0;

        public IEnumerable<Unit> GetValidUnits() => GetUnits().Where(u => u != null);

        public IReadOnlyList<Unit> GetUnits() => BattleGroupSystem.GetUnits(this);

        public virtual void ReplaceUnit(Unit oldUnit, Unit newUnit, int index=-1) => BattleGroupSystem.ReplaceUnit(this, oldUnit, newUnit, index);

        public virtual void AddUnit(Unit u) => BattleGroupSystem.AddUnit(this, u);

        public virtual void RemoveUnit(Unit u) => BattleGroupSystem.RemoveUnit(this, u);

        public BattleTeam GetBattleTeam() => new BattleTeam(this, BattleGroupSystem.GetUnits(this).ToArray());

        public void OnBattleFinished(TurnBattle battle, BattleHeader BattleHeader, BattleTurnEvent[] Turns)
        {
            BattleGroupSystem.OnBattleFinished(this, battle, BattleHeader, Turns);
        }

        public void OnBattleStarted(TurnBattle battle)
        {
            this.BattleID = battle.ID;
        }

        public override string ToString()
        {
            return $"<Party Id={Id} Index={PartyIndex} Owner={OwnerID}>";
        }

        public ServerPacket GetStatusUpdatePacket() => new PartyStatusUpdatePacket(this);
    }
}

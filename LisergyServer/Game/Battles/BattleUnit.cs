using Game.Entity;
using System;

namespace Game.Battles
{
    [Serializable]
    public class BattleUnit : IComparable<BattleUnit>
    {
        private Unit _unit;

        [NonSerialized]
        private BattleTeam _team;

        public Unit Unit { get => _unit; set => _unit = value; }

        public UnitStats Stats { get => Unit.Stats; }

        public BattleTeam Team { get => _team; set => _team = value; }

        public int ActionTime { get; private set; }

        public bool Dead { get => Stats.HP == 0; }

        public BattleUnit(BattleTeam team, Unit unit)
        {
            this.Team = team;
            this.Unit = unit;
            this.ActionTime = GetDelay();
        }

        public int GetDelay() => Math.Max(1, 30 - this.Unit.Stats.Speed);

        public void UpdateActionTime()
        {
            ActionTime += GetDelay();
        }

        public int CompareTo(BattleUnit obj)
        {
            if (obj == this)
                return 0;
            if (obj.ActionTime >= this.ActionTime)
                return -1;
            else 
                return 1;
        }

        public override string ToString()
        {
            return $"<BattleUnit AT={ActionTime} {Unit.ToString()}>";
        }
    }
}

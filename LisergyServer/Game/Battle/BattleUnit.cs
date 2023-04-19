using Game.Battler;
using Game.DataTypes;
using Game.Player;
using System;

namespace Game.Battle
{
    [Serializable]
    public class BattleUnit : IComparable<BattleUnit>
    {
        private Unit _unitReference;

        [NonSerialized]
        private BattleTeam _team;

        public BattleTeam Team { get => _team; set => _team = value; }

        public int RT { get; set; }

        public bool Dead => _unitReference.HP <= 0;
        public GameId UnitID => UnitReference.Id;
        public Unit UnitReference { get => _unitReference; set => _unitReference = value; }

        public BattleUnit(PlayerEntity owner, BattleTeam team, Unit unit)
        {
            Team = team;
            UnitReference = unit;
            RT = GetMaxRT();
        }

        public int GetMaxRT()
        {
            return Math.Max(1, 100 - _unitReference.Speed);
        }

        public void IncreaseRT(int delay)
        {
            RT += delay;
        }


        public int CompareTo(BattleUnit obj)
        {
            return obj == this ? 0 : obj.RT >= RT ? -1 : 1;
        }

        public override string ToString()
        {
            return $"<BattleUnit RT={RT} {UnitReference}>";
        }
    }
}

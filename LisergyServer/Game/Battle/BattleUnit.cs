using Game.DataTypes;
using Game.Entity;
using Game.Entity.Entities;
using GameData.buffs;
using System;
using System.Collections.Generic;

namespace Game.Battles
{
    [Serializable]
    public class BattleUnit : IComparable<BattleUnit>
    {
        private Unit _unitReference;
        public bool Controlled = false;

        [NonSerialized]
        private BattleTeam _team;

        public BattleTeam Team { get => _team; set => _team = value; }

        public int RT { get; set; }

        public bool Dead { get => _unitReference.HP == 0; }
        public GameId UnitID { get => UnitReference.Id; }
        public Unit UnitReference { get => _unitReference; set { _unitReference = value; } }

        public BattleUnit(PlayerEntity owner, BattleTeam team, Unit unit)
        {
            this.Team = team;
            UnitReference = unit;
            this.RT = GetMaxRT();
        }

        public int GetMaxRT() => Math.Max(1, 100 - _unitReference.Speed);

        public void IncreaseRT(int delay)
        {
            RT += delay;
        }


        public int CompareTo(BattleUnit obj)
        {
            if (obj == this)
                return 0;
            if (obj.RT >= this.RT)
                return -1;
            else
                return 1;
        }

        public override string ToString()
        {
            return $"<BattleUnit RT={RT} {UnitReference}>";
        }
    }
}

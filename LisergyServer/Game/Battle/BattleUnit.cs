using Game.Entity;
using Game.TacticsBattle;
using System;

namespace Game.Battles
{
    [Serializable]
    public class BattleUnit : MovableWorldEntity, IComparable<BattleUnit>
    {
        public static int RT_PER_TILE_MOVED = 2;

        public Unit _unitReference;

        public bool Moved = false;
        public bool Actioned = false;
        public bool UsedSkill = false;

        [NonSerialized]
        private BattleTeam _team;

        public UnitStats Stats { get => _unitReference.Stats; }

        public BattleTeam Team { get => _team; set => _team = value; }

        public Unit GetUnitReference()
        {
            return _unitReference;
        }

        public int RT { get; set; }

        public bool Dead { get => Stats.HP == 0; }
        public string UnitID { get => _unitReference.Id; }

        public BattleUnit(PlayerEntity owner, BattleTeam team, Unit unit): base(owner)
        {
            this.Team = team;
            _unitReference = unit;
            this.RT = GetMaxRT();
        }

        public int GetMaxRT() => Math.Max(1, 100 - this.Stats.Speed);

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
            return $"<BattleUnit ID={_unitReference.Id} Name={_unitReference.Name} RT={RT}>";
        }

        public override TimeSpan GetMoveDelay()
        {
            return TimeSpan.FromSeconds(0.5);
        }

        public override byte GetLineOfSight()
        {
            return 3;
        }
    }
}

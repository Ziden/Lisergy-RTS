using Game.Entity;
using Game.TacticsBattle;
using System;

namespace Game.Battles
{
    [Serializable]
    public class BattleUnit : MovableWorldEntity, IComparable<BattleUnit>
    {
        public static int RT_PER_TILE_MOVED = 2;

        public bool Moved = false;
        public bool Actioned = false;
        public bool UsedSkill = false;

        [NonSerialized]
        private BattleTeam _team;

        [NonSerialized]
        private UnitStats _stats;

        private string _unitID;
        private string _unitName;

        public UnitStats Stats { get => _stats; }

        public BattleTeam Team { get => _team; set => _team = value; }

        public int RT { get; set; }

        public bool Dead { get => Stats.HP == 0; }
        public string UnitID { get => _unitID; }

        public BattleUnit(PlayerEntity owner, BattleTeam team, Unit unit): base(owner)
        {
            this.Team = team;
            _stats = unit.Stats;
            _unitName = unit.Name;
            _unitID = unit.Id;
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
            return $"<BattleUnit ID={_unitID} Name={_unitName} RT={RT}>";
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

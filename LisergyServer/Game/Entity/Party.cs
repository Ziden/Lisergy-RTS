using Game.Events;
using Game.Events.ServerEvents;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entity
{
    [Serializable]
    public class Party : MovableWorldEntity
    {
        private byte _partyIndex;
        private Unit[] _units = new Unit[4] { null, null, null, null };
         
        [NonSerialized]
        private CourseTask _course;

        public byte PartyIndex { get => _partyIndex; }
        public override TimeSpan GetMoveDelay() => TimeSpan.FromSeconds(1);
        public CourseTask Course { get => _course; set => _course = value; }

        public Party(PlayerEntity owner, byte partyIndex) : base(owner)
        {
            _partyIndex = partyIndex;
        }

        private byte GetLineOfSight()
        {
            return _units.Where(u => u != null).Select(u => StrategyGame.Specs.Units[u.SpecID].LOS).Max();
        }

        public IEnumerable<Unit> GetUnits()
        {
            return _units.Where(u => u != null);
        }

        public virtual void SetUnit(Unit u, int index)
        {
            _units[index] = u;
        }

        public virtual void AddUnit(Unit u)
        {
            if(u.Party != null && u.Party != this)
                u.Party.RemoveUnit(u);
            var freeIndex = Array.IndexOf(_units, null);
            SetUnit(u, freeIndex);
            this.LineOfSight = GetLineOfSight();
        }

        public virtual void RemoveUnit(Unit u)
        {
            var index = Array.IndexOf(_units, u);
            _units[index] = null;
            this.LineOfSight = GetLineOfSight();
        }

        public override Tile Tile
        {
            get { return base.Tile; }
            set
            {
                if (base.Tile != null)
                    base.Tile.Parties.Remove(this);

                base.Tile = value;
                base.Tile.Parties.Add(this);
            }
        }

        public override string ToString()
        {
            return $"<Party Id={PartyIndex} Owner={OwnerID}>";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entity
{
    [Serializable]
    public class Party : WorldEntity
    {
        private byte _partyIndex;
        private Unit[] _units = new Unit[4] { null, null, null, null };

        public byte GetLineOfSight()
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

        public Party(PlayerEntity owner, byte partyIndex): base(owner)
        {
            _partyIndex = partyIndex;
        }

        public virtual void AddUnit(Unit u)
        {
            if(u.Party != null && u.Party != this)
                u.Party.RemoveUnit(u);
            var freeIndex = Array.IndexOf(_units, null);
            SetUnit(u, freeIndex);
        }

        public virtual void RemoveUnit(Unit u)
        {
            var index = Array.IndexOf(_units, u);
            _units[index] = null;
        }

        public byte PartyIndex { get => _partyIndex; }
    }
}

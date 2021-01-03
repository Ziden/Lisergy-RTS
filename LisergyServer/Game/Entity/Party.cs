using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entity
{
    [Serializable]
    public class Party : WorldEntity
    {
        private byte _partySlotId;
        private Unit[] _units = new Unit[4] { null, null, null, null };

        public byte GetLineOfSight()
        {
            return _units.Where(u => u != null).Select(u => StrategyGame.Specs.Units[u.SpecID].LOS).Max();
        }

        public Party(PlayerEntity owner, byte partyID): base(owner)
        {
            _partySlotId = partyID;
        }

        public void AddUnit(Unit u)
        {
            if(u.Party != null && u.Party != this)
                u.Party.RemoveUnit(u);
            var freeIndex = Array.IndexOf(_units, null);
            _units[freeIndex] = u;
        }

        public void RemoveUnit(Unit u)
        {
            var index = Array.IndexOf(_units, u);
            _units[index] = null;
        }

        public byte PartyID { get => _partySlotId; }
        public Unit [] Units { get => _units; }
    }
}

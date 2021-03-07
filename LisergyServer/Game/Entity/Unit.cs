using Game.Entity;
using GameData.Specs;
using System;

namespace Game
{
    [Serializable]
    public class Unit
    {
        private byte _partyId;

        [NonSerialized]
        private Party _party;

        public string Id { get; private set; }
        public ushort SpecId { get; private set; }
        public UnitStats Stats { get; private set; }
        public UnitSpec Spec { get => StrategyGame.Specs.Units[this.SpecId]; }

        public Unit(ushort unitSpecId)
        {
            this.SpecId = unitSpecId;
            this.Id = Guid.NewGuid().ToString();
            this.Stats = new UnitStats();
            this.Stats.SetStats(this.Spec.Stats);
        }

        public Party Party
        {
            get => _party;
            set {
                _party = value;
                if(value != null)
                    _partyId = value.PartyIndex;
            }
        }

        public override string ToString()
        {
            return $"<Unit spec={SpecId}/>";
        }
    }
}

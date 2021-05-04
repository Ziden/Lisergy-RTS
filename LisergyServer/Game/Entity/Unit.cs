using Game.Entity;
using GameData.Specs;
using System;

namespace Game
{
    [Serializable]
    public class Unit
    {
        private byte _partyId = byte.MaxValue;

        [NonSerialized]
        private Party _party;

        public string  Name { get; set; }
        public string Id { get; protected set; }
        public ushort SpecId { get; private set; }
        public UnitStats Stats { get; private set; }
        public UnitSpec Spec { get => StrategyGame.Specs.Units[this.SpecId]; }

        public Unit(ushort unitSpecId)
        {
            this.Name = "Unamed";
            this.SpecId = unitSpecId;
            this.Id = Guid.NewGuid().ToString();
            this.Stats = new UnitStats();
        }

        public void SetSpecStats()
        {
            this.Stats.SetStats(Spec.Stats);
        }

        public Party Party
        {
            get => _party;
            set {
                _party = value;
                if (value != null)
                    _partyId = value.PartyIndex;
                else
                    _partyId = byte.MaxValue;
            }
        }

        public byte PartyId { get => _partyId; }

        public override string ToString()
        {
            return $"<Unit name={Name} spec={SpecId}/>";
        }
    }
}

using Game.Entity;
using GameData.Specs;
using System;

namespace Game
{
    [Serializable]
    public class Unit
    {
        private byte _partyID;

        [NonSerialized]
        private Party party;

        public string Id { get; private set; }
        public ushort SpecID { get; private set; }
        public UnitStats Stats { get; private set; }
        public UnitSpec Spec { get => StrategyGame.Specs.Units[this.SpecID]; }

        public Unit(ushort unitSpecID)
        {
            this.SpecID = unitSpecID;
            this.Id = Guid.NewGuid().ToString();
            this.Stats = new UnitStats();
            this.Stats.SetStats(this.Spec.Stats);
        }

        public Party Party
        {
            get => party;
            set {
                party = value;
                if(value != null)
                    _partyID = value.PartyIndex;
            }
        }

        public override string ToString()
        {
            return $"<Unit spec={SpecID}/>";
        }
    }
}

using Game.Entity;
using GameData.Specs;
using System;

namespace Game
{
    [Serializable]
    public class Unit
    {
        [NonSerialized]
        private byte _partyId = byte.MaxValue;

        [NonSerialized]
        private Party _party;

        public string Name { get; set; }
        public GameId Id { get; protected set; }
        public ushort SpecId { get; private set; }
        public UnitStats Stats { get; private set; }

        public Unit(ushort unitSpecId)
        {
            this.Name = "NoName";
            this.SpecId = unitSpecId;
            this.Id = Guid.NewGuid();
            this.Stats = new UnitStats();
        }


        public void SetSpecStats()
        {
            this.Stats.SetStats(StrategyGame.Specs.Units[this.SpecId].Stats);
            HealAll();
        }

        public void HealAll()
        {
            this.Stats.HP = this.Stats.MaxHP;
            this.Stats.MP = this.Stats.MaxMP;
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

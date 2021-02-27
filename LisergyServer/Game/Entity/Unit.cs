using Game.Entity;
using GameData.Specs;
using System;
using System.Collections.Generic;

namespace Game
{
    [Serializable]
    public class Unit : Ownable
    {
        private byte _partyID;

        [NonSerialized]
        private Party party;

        public string Id { get; private set; }
        public ushort SpecID { get; private set; }
        public UnitStats Stats { get; private set; }
        public UnitSpec UnitSpec { get => StrategyGame.Specs.Units[this.SpecID]; }

        public Unit(ushort unitSpecID, PlayerEntity owner, string id = null): base(owner)
        {
            this.Owner = owner;
            this.SpecID = unitSpecID;
            if (id == null)
                this.Id = Guid.NewGuid().ToString();
            else
                this.Id = id;

            this.Stats = new UnitStats();
            this.Stats.SetStats(this.UnitSpec.Stats);
        }

        public Party Party
        {
            get => party;
            set {
                party = value;
                if(value != null)
                {
                    _partyID = value.PartyIndex;
                } 
            }
        }

        public override string ToString()
        {
            return $"<Unit spec={SpecID} owner={Owner}/>";
        }
    }
}

using Game.Entity;
using System;

namespace Game
{
    [Serializable]
    public class Unit : Ownable
    {
        private string _id;
        private ushort _specID;
        private byte _partyID;

        [NonSerialized]
        private Party party;

        public string Id { get => _id; private set => _id = value; }
        public ushort SpecID { get => _specID; private set => _specID = value; }

        public Unit(ushort unitSpecID, PlayerEntity owner, string id = null): base(owner)
        {
            this.Owner = owner;
            this.SpecID = unitSpecID;
            if (id == null)
                this.Id = Guid.NewGuid().ToString();
            else
                this.Id = id;
        }

        public Party Party
        {
            get => party;
            set {
                party = value;
                if(value != null)
                {
                    _partyID = value.PartyID;
                } 
            }
        }

        public override string ToString()
        {
            return $"<Unit spec={SpecID} owner={Owner}/>";
        }
    }
}

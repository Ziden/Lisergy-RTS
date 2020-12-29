using System;

namespace Game
{
    [Serializable]
    public class Unit : WorldEntity
    {
        public ushort SpecID;

        public Unit(ushort unitSpecID, PlayerEntity owner, string id = null) : base(owner) {
            this.Owner = owner;
            this.SpecID = unitSpecID;
            if (id == null)
                this.Id = Guid.NewGuid().ToString();
            else
                this.Id = id;
        }

        public override string ToString()
        {
            return $"<Unit spec={SpecID} owner={Owner}/>";
        }
    }
}

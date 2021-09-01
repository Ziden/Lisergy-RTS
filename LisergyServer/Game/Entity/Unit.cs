using Game.Entity;
using System;

namespace Game
{
    [Serializable]
    public class Unit
    {
        public string Name { get; set; }
        public string Flavour { get; set; }
        public string Id { get; private set; }
        public UnitStats Stats { get; private set; }

        public string Sprite;

        public ushort Level;
        public Rarity Rarity;
        public string PartyID;

        public Unit()
        {
            this.Id = Guid.NewGuid().ToString();
            Stats = new UnitStats();
        }
        public override string ToString()
        {
            return $"<Unit Id={Id}/>";
        }
    }
}

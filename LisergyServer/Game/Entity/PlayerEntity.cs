
using Game.Battles;
using Game.Entity;
using Game.Events;
using System;
using System.Collections.Generic;

namespace Game
{
    public abstract class PlayerEntity
    {
        public string UserID;

        public HashSet<Unit> Units = new HashSet<Unit>();

        private Dictionary<string, TurnBattle> _currentBattles = new Dictionary<string, TurnBattle>();
        private Dictionary<string, Party> _parties = new Dictionary<string, Party>();


        public PlayerEntity()
        {
            this.UserID = Guid.NewGuid().ToString();
        }

        public Party CreateParty(Unit [] units)
        {
            var party = new Party();
            party.PartyID = Guid.NewGuid().ToString();
            party.Units.AddRange(units);
            _parties[party.PartyID] = party;
            foreach (var assigned in units)
                assigned.PartyID = party.PartyID;
            return party;
        }

        public Party GetParty(string partyId)
        {
            if (!_parties.ContainsKey(partyId))
                return null;
            return _parties[partyId];
        }

        public PlayerEntity(string id)
        {
            this.UserID = id;
        }

        public abstract void Send<EventType>(EventType ev) where EventType : BaseEvent;

        public abstract bool Online();

        public override string ToString()
        {
            return $"<Player id={UserID.ToString()}>";
        }
    }
}

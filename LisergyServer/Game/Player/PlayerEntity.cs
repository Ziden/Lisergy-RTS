
using Game.Battles;
using Game.Entity;
using Game.Events;
using Game.InfiniteDungeon;
using System;
using System.Collections.Generic;

namespace Game
{
    public abstract class PlayerEntity
    {
        public string UserID;
        private Dictionary<string, Unit> _units = new Dictionary<string, Unit>();

        public InfinityDungeon RunningDungeon;

        public IEnumerable<Unit> Units => _units.Values;

        public void AddUnit(Unit u)
        {
            _units[u.Id] = u;
        }
  
        public PlayerEntity()
        {
            this.UserID = Guid.NewGuid().ToString();
        }

        public Unit GetUnit(string unitId)
        {
            if (unitId == null || !_units.ContainsKey(unitId))
                return null;
            return _units[unitId];
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

using Game.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public abstract class PlayerEntity
    {
        public string UserID;

        public HashSet<Building> Buildings = new HashSet<Building>();

        public PlayerEntity()
        {
            this.UserID = Guid.NewGuid().ToString();
        }

        public PlayerEntity(string id)
        {
            this.UserID = id;
        }

        public void Build(byte id, Tile t)
        {
            var building = new Building(id, this, t);
            t.Building = building;
            Log.Debug($"Player {UserID} built {id}");
        }

        public abstract void Send<EventType>(EventType ev) where EventType : GameEvent;

        public abstract bool Online();

        public override string ToString()
        {
            return $"<Player id={UserID.ToString()}>";
        }
    }
}

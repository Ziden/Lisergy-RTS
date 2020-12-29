using Game.Events;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public abstract class PlayerEntity
    {
        public string UserID;

        public HashSet<Unit> Units = new HashSet<Unit>();
        public HashSet<Building> Buildings = new HashSet<Building>();
        public HashSet<Tile> VisibleTiles = new HashSet<Tile>();

        public PlayerEntity()
        {
            this.UserID = Guid.NewGuid().ToString();
        }

        public PlayerEntity(string id)
        {
            this.UserID = id;
        }

        public Building GetCenter()
        {
            return Buildings.First(b => b.BuildingID == StrategyGame.Specs.InitialBuilding);
        }

        public void RecruitUnit(ushort unitSpecId)
        {
            var unit = new Unit(unitSpecId, this);
            this.Units.Add(unit);

            var center = GetCenter().Tile.GetNeighbor(Direction.EAST);
            center.TeleportUnit(unit);
            Log.Debug($"{UserID} recruited {unitSpecId} at {center}");
        }

        public void Build(byte id, Tile t)
        {
            var building = new Building(id, this);
            this.Buildings.Add(building);
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

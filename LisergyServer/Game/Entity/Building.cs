using Game.Entity;
using GameData;
using System;

namespace Game
{
    public class Building : ExploringEntity
    {
        public byte SpecID { get; private set; }
        
        public Building(byte id, PlayerEntity owner): base(owner)
        {
            this.SpecID = id;
            this.LineOfSight = GetSpec().LOS;
        }

        public override Tile Tile
        {
            get { return base.Tile; }
            set
            {
                if (value.Building != this)
                    throw new Exception("Use tile.Building = value to set a building into a tile");
                base.Tile = value;
            }
        }

        protected override void SendVisibilityPackets(Tile from, Tile to)
        {
            /*
             * Buildings have their visibility sent as a byte inside tile reffering to the spec 
             * and a string reffering to the owner.
             */
        }

        public BuildingSpec GetSpec()
        {
            return StrategyGame.Specs.Buildings[SpecID];
        }

        public override string ToString()
        {
            return $"<Building tile={Tile} id={SpecID} Owner={Owner}>";
        }
    }
}

using Game.Entity;
using GameData;
using System;

namespace Game
{
    [Serializable]
    public class Building : StaticEntity
    {
        public ushort SpecID { get; private set; }

        public override byte GetLineOfSight()
        {
            var spec = GetSpec();
            return GetSpec().LOS;
        }

        public Building(ushort id, PlayerEntity owner): base(owner)
        {
            this.SpecID = id;
        }

        public virtual BuildingSpec GetSpec()
        {
            return StrategyGame.Specs.Buildings[SpecID];
        }

        public override string ToString()
        {
            return $"<Building x={_x} y={_y} id={SpecID} Owner={Owner}>";
        }
    }
}

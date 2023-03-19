using Game.Entity;
using Game.Entity.Components;
using GameData;
using System;

namespace Game
{
    [Serializable]
    public class Building : StaticEntity
    {

        private ushort _specId;

        public ushort SpecID { get => _specId; set
            {
                _specId = value;
                this.Components.Add<EntityExplorationComponent>().LineOfSight = GetSpec().LOS;
            }  
        }

        public Building(PlayerEntity owner): base(owner)
        {
           
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

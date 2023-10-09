using Game.ECS;
using System.Collections.Generic;

namespace Game.Systems.Tile
{
    public class TileHabitants : IReferenceComponent
    {
        public List<IEntity> EntitiesIn = new List<IEntity>();
        public IEntity Building;

        public override string ToString()
        {
            return $"<TileHabitantsComponent EntitiesIn={EntitiesIn.Count} Building={Building.EntityId}>";
        }
    }
}
